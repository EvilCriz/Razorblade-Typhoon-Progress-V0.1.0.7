using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace RazorbladeTyphoonProgress.Projectiles;

//[RU]: Класс, реализующий снаряд для основного предмета.
//------------------------------------------
//[EN]: Class implementing the projectile for the main item.
public class ProjectileRazorbladeTyphoonProgress : ModProjectile
{
	//[RU]: Переопределение расположения текстуры данного предмета в проекте
	//------------------------------------------
	//[EN]: Overriding the placement of the texture for this item in the project
	public override string Texture => "RazorbladeTyphoonProgress/Content/Projectiles/" + nameof(ProjectileRazorbladeTyphoonProgress);
	
	//[RU]: Данная строка указывает, что локализованный текст по ключу "DisplayName" не должен создаваться в файлах локализации
	//------------------------------------------
	//[EN]: This line indicates that the localized text with the key "DisplayName" should not be generated in the localization files.
    public override LocalizedText DisplayName => LocalizedText.Empty;
	
	//[RU]: Переменная, хранящая в себе время, сколько снаряд не будет искать другие цели
	//[RU]: Значение присваивается при столкновении с блоками или при нанесении урона NPC
	//------------------------------------------
	//[EN]: Variable storing the time during which the projectile will not seek other targets
	//[EN]: The value is assigned upon collision with blocks or when dealing damage to NPCs
    private int Expectation = 0;
	
	//[RU]: Переменная, хранящая максимальный радиус поиска NPC (радиус расчитывается от центра снаряда)
	//------------------------------------------
	//[EN]: Variable storing the maximum radius for searching NPCs (the radius is calculated from the center of the projectile)
    private float MaxDetectRadius = 700f;
	
	//[RU]: Переменная, хранящая скорость снаряда
	//------------------------------------------
	//[EN]: Variable storing the projectile speed
	private float ProjSpeed = 12f;
	
	//[RU]: Вспомогательный класс "EffectsSystem" для категории "Дополнительные эффекты"
	//[RU]: См. CustomClasses/EffectsSystem.cs
	//------------------------------------------
	//[EN]: Auxiliary class "EffectsSystem" for the "Additional Effects" category
	//[EN]: See CustomClasses/EffectsSystem.cs
    private CustCl.EffectsSystem EffectsSystem = CustCl.EffectsSystem.CreateEffectSystem();
	
	//[RU]: Перечисление, хранящее в себе список всех категорий улучшений
	//[RU]: Определен в классе Common/Player/PlayerSave.cs
	//------------------------------------------
	//[EN]: Enumeration containing a list of all improvement categories
	//[EN]: Defined in the class Common/Player/PlayerSave.cs
    private Common.PlayerSave.LevelType LevelType = Common.PlayerSave.LevelType.Nan;
	
	//[RU]: Переменная, хранящая в себе значение, первый ли раз вызывается метод AI() для данного снаряда
	//------------------------------------------
	//[EN]: Variable storing whether the AI() method is called for this projectile for the first time
	private bool IsFirstAI = true;
	
    public override void SetDefaults()
    {
        Projectile.aiStyle = 0;
        AIType = 0;
        Projectile.timeLeft = 10 * 60;
        Projectile.alpha = 0;
        Projectile.light = 1f;
		
		Projectile.width = 50;
		Projectile.height = 50;
        
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.ignoreWater = true;
        
        Projectile.DamageType = DamageClass.Magic;

		Projectile.penetrate = 1;		
    }

	//[RU]: Метод AI() определен таким образом, чтобы заставлять снаряд каждый вызов данного метода искать ближайшего NPC в определенном радиусе
	//[RU]: Если данный NPC найден - следовать за ним, в противном случае - лететь по старой траектории
	//------------------------------------------
	//[EN]: The AI() method is defined in such a way as to make the projectile, with each call of this method, search for the nearest NPC within a specific radius.
	//[EN]: If this NPC is found, follow it; otherwise, continue flying along the old trajectory.
    public override void AI()
    {
		//[RU]: Проверка, отвечающая за то, чтобы код AI() не вызывался у игроков, не являющихся владельцами данного снаряда
		//------------------------------------------
		//[EN]: Check ensuring that the AI() code is not called for players who are not the owners of this projectile.
        if(Projectile.owner != Main.myPlayer) return;
        
		//[RU]: Если метод AI() вызвался впервые, добавляем ему количество атак снаряда до исчезновения
		//[RU]: Дополнительное количество снарядов берется исходя из уровня персонажа в категории "Дополнительное количество атак снаряда"
		//------------------------------------------
		//[EN]: If the AI() method is called for the first time, add the number of projectile attacks before disappearing.
		//[EN]: The additional number of projectiles is determined based on the character's level in the "Additional Projectile Attacks" category.
		if(IsFirstAI)
		{
			IsFirstAI = false;
			Projectile.penetrate += Main.LocalPlayer.GetModPlayer<Common.PlayerSave>().PlayerLevelPenetrate;
			Projectile.maxPenetrate = Projectile.penetrate;
		}
		
		//[RU]: Если переменная Expectation больше 0:
		//[RU]: Уменьшаем значение переменной Expectation и выходим из метода AI();
		//[RU]: Тем самым мы не даем снаряду искать новые цели, пока переменная Expectation больше 0
		//------------------------------------------
		//[EN]: If the Expectation variable is greater than 0:
		//[EN]: Decrease the value of the Expectation variable and exit the AI() method;
		//[EN]: This way, we prevent the projectile from seeking new targets as long as the Expectation variable is greater than 0.
        if(Expectation > 0)
        {
            Expectation--;
            return;
        }

		//[RU]: Переменная, для хранения ближайшего найденного NPC
		//------------------------------------------
		//[EN]: Variable for storing the nearest found NPC
        NPC closestNPC = null;
		
		//[RU]: Вычисляем квадрат поиска исходя из максимального радиуса поиска NPC
		//------------------------------------------
		//[EN]: Calculating the search square based on the maximum NPC search radius
        float sqrMaxDetectDistance = MaxDetectRadius * MaxDetectRadius;

		//[RU]: Перебор всех существующих NPC в мире
		//------------------------------------------
		//[EN]: Iterating through all existing NPCs in the world
        for (int k = 0; k < Main.maxNPCs; k++) 
        {
            NPC target = Main.npc[k];
			
			//[RU]: Проверяем, может ли NPC из текущей итерации быть атакованным (является ли он вражеским)
			//------------------------------------------
			//[EN]: Checking whether the NPC from the current iteration can be attacked (is it an enemy)
            if (target.CanBeChasedBy()) 
			{
				//[RU]: Вычисляем дистанцию от центра снаряда до центра NPC из текущей итерации
				//------------------------------------------
				//[EN]: Calculating the distance from the center of the projectile to the center of the NPC from the current iteration
                float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);
				
				//[RU]: Проверяем, находится ли NPC в зоне квадрата поиска 
				//------------------------------------------
				//[EN]: Checking if the NPC is within the search square zone
                if (sqrDistanceToTarget < sqrMaxDetectDistance) 
				{
					//[RU]: Ограничиваем новый квадрат поиска NPC квадратом от центра снаряда до центра NPC из текущей итерации
					//------------------------------------------
					//[EN]: Limiting the new search square for NPC to the square from the center of the projectile to the center of the NPC from the current iteration
                    sqrMaxDetectDistance = sqrDistanceToTarget;
                    closestNPC = target;
                }
            }
        }

		//[RU]: Если NPC найден - вычислить векторную скорость к данному NPC
		//------------------------------------------
		//[EN]: If NPC is found - calculate the vector speed towards this NPC
        if (closestNPC != null)
            Projectile.velocity = (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * ProjSpeed;
	
		Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 33);
		
		//[RU]: Заставляем вращаться снаряд
		//------------------------------------------
		//[EN]: Make the projectile rotate
        Projectile.rotation += 0.4f * (float)Projectile.direction;
    }

	//[RU]: Метод, определяющий, некоторые настройки столкновения снаряда с блоками, и то будет ли вообще этот снаряд с ними сталкиваться
	//------------------------------------------
	//[EN]: Method determining some collision settings for the projectile with blocks, and whether the projectile will collide with them at all
    public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
    {	
		//[RU]: Устаналиваем, что столкновение с плиткой будет считаться стлокновением, только если снаряд столкнулся часть 16х16..
		//[RU]: От центра снаряда. Вне зависимости от установленного размера самого снаряда
		//------------------------------------------
		//[EN]: Set that collision with tiles will be considered a collision only if the projectile collides with a 16x16..
		//[EN]: Part from the center of the projectile. Regardless of the projectile's actual size.
		width = 16;
		height = 16;
		
		//[RU]: Получаем экземпляр класса PlayerSave игрока, который является владельцем данного снаряда.
		//[RU]: Класс PlayerSave содержит все данные о текущем уровне, нанесенном уроне и т.д. для каждой категории
		//[RU]: Исключение: максимальный уровень категории "Дополнительные эффекты" хранится в классе "EffectsSystem"
		//[RU]: См. Common/Player/PlayerSave.cs
		//------------------------------------------
		//[EN]: Obtain an instance of the PlayerSave class for the player who owns this projectile.
		//[EN]: The PlayerSave class contains all data about the current level, damage dealt, etc. for each category.
		//[EN]: Exception: The maximum level of the "Additional Effects" category is stored in the "EffectsSystem" class.
		//[EN]: See Common/Player/PlayerSave.cs
        Common.PlayerSave playerSave = Main.player[Projectile.owner].GetModPlayer<Common.PlayerSave>();
		
		//[RU]: Получаем текущий уровень владельца снаряда из категории "Дополнительные эффекты"
		//------------------------------------------
		//[EN]: Obtain the current level of the projectile owner from the "Additional Effects" category
        int lvlEffect = playerSave.PlayerLevelEffect;
		
		//[RU]: Получает на каком уровне категори "Дополнительные эффекты" открывается доступ к усиление "Снаряды проходят сквозь блоки"
		//[RU]: См. в классе CustomClasses/EffectsSystem.cs
		//------------------------------------------
		//[EN]: Obtain at which level the "Additional Effects" category unlocks access to the "Projectiles pass through blocks" enhancement.
		//[EN]: See in the CustomClasses/EffectsSystem.cs class.
        int lvlNoCollide = EffectsSystem.GetNoCollideLevel();
		
		//[RU]: Проверяем, что такой эффект определен в классе EffectsSystem и уровень игрока больше или равен lvlNoCollide
		//------------------------------------------
		//[EN]: Check that such an effect is defined in the EffectsSystem class and the player's level is greater than or equal to lvlNoCollide.
        if(lvlNoCollide != -1 && lvlEffect >= lvlNoCollide)
        {
			//[RU]: Установка значение fallThrough = true и возврат значения false предотвратит столкновение с блоками.
			//[RU]: В этом случае вызов метода OnTileCollide (см. ниже) не произойдет
			//------------------------------------------
			//[EN]: Setting fallThrough = true and returning false will prevent collision with blocks.
			//[EN]: In this case, the OnTileCollide method call (see below) will not occur.
            fallThrough = true;
            return false;
        }
               
        return true;
    }

	//[RU]: Метод, вызывающийся при столкновении снаряда с блоками
	//------------------------------------------
	//[EN]: Method called upon the projectile's collision with blocks
    public override bool OnTileCollide(Vector2 oldVelocity) 
    {
		//[RU]: Установка Expectation на 12, что означает, что снаряд не будет искать новые цели примерно 1/6 секунды
		//------------------------------------------
		//[EN]: Setting Expectation to 12, which means that the projectile will not seek new targets for approximately 1/6th of a second.
        Expectation = 12;
		
        Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
        if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon) {
            Projectile.velocity.X = -oldVelocity.X;
        }

        if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon) {
            Projectile.velocity.Y = -oldVelocity.Y;
        }
        
        return false;
    }

	//[RU]: Метод, определяющий, может ли данный снаряд при столкновении с NPC атаковать его.
	//[RU]: Возврат значения false предотвратит вызов OnHitNPC
	//[RU]: Если результат target.CanBeChasedBy() вернет true - NPC может быть атакованным (он является вражеским)
	//[RU]: В этом случае мы возвращаем null, что эквивалентно значению по умолчанию.
	//[RU]: В противном случае мы запрещаем атаковать данного NPC, вернув false
	//------------------------------------------
	//[EN]: Method determining whether this projectile can attack an NPC upon collision.
	//[EN]: Returning false will prevent the OnHitNPC call.
	//[EN]: If the result of target.CanBeChasedBy() is true - NPC can be attacked (it is an enemy).
	//[EN]: In this case, we return null, which is equivalent to the default value.
	//[EN]: Otherwise, we disallow attacking this NPC by returning false.
    public override bool? CanHitNPC(NPC target)
		=> target.CanBeChasedBy() ? null : false;

	//[RU]: Метод, вызывается каждый раз, когда данный снаряд атаковал NPC
	//------------------------------------------
	//[EN]: Method called every time this projectile has attacked an NPC
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
		//[RU]: Предотвращаем вызов данного кода для игроков, НЕ являющихся владельцами данного снаряда
		//------------------------------------------
		//[EN]: Preventing the execution of this code for players who are NOT the owners of this projectile.
        if(Projectile.owner != Main.myPlayer) return;

		//[RU]: Установка Expectation на 12, что означает, что снаряд не будет искать новые цели примерно 1/6 секунды
		//------------------------------------------
		//[EN]: Setting Expectation to 12, which means that the projectile will not seek new targets for approximately 1/6th of a second.
        Expectation = 12;

		//[RU]: Получаем экземпляр класса PlayerSave локального игрока.
		//[RU]: (В данном случае мы можем использовать Main.LocalPlayer, т.к. этот код вызовется только у владельца снаряда)
		//[RU]: (Благодаря проверке выше)
		//[RU]: Класс PlayerSave содержит все данные о текущем уровне, нанесенном уроне и т.д. для каждой категории
		//[RU]: Исключение: максимальный уровень категории "Дополнительные эффекты" хранится в классе "EffectsSystem"
		//[RU]: См. Common/Player/PlayerSave.cs
		//------------------------------------------
		//[EN]: Obtain an instance of the PlayerSave class for the local player.
		//[EN]: (In this case, we can use Main.LocalPlayer, as this code will only be called for the projectile owner)
		//[EN]: (Thanks to the previous check)
		//[EN]: The PlayerSave class contains all data about the current level, damage dealt, etc. for each category.
		//[EN]: Exception: The maximum level of the "Additional Effects" category is stored in the "EffectsSystem" class.
		//[EN]: See Common/Player/PlayerSave.cs
        Common.PlayerSave ps = Main.LocalPlayer.GetModPlayer<Common.PlayerSave>();
		
		//[RU]: Получаем уровень игрока в категории "Дополнительные эффекты"
		//[RU]: Если он превышает максимальный уровень категории "Дополнительные эффекты" - возвращаем максимальный уровень данной категории.
		//[RU]: Сделано для предотвращения ошибок, в случае уменьшения максимального уровня данной категории в будущем
		//------------------------------------------
		//[EN]: Obtain the player's level in the "Additional Effects" category.
		//[EN]: If it exceeds the maximum level of the "Additional Effects" category, return the maximum level of this category.
		//[EN]: Done to prevent errors in case of future reduction of the maximum level of this category.
        int level = ps.PlayerLevelEffect <= EffectsSystem.MaxLevel + 1 ? 
            ps.PlayerLevelEffect : EffectsSystem.MaxLevel + 1;

		//[RU]: Перебираем все открытые на текущем уровне усиления из категории "Дополнительные эффекты"
		//[RU]: Если переменная isAddNPCTarget == true для усиления из данной итерации, это означает,
		//[RU]: Что предполагается наложение данного баффа на вражеского NPC.
		//[RU]: В этом случае мы добавляем бафф NPC
		//------------------------------------------
		//[EN]: Iterate through all open enhancements at the current level from the "Additional Effects" category.
		//[EN]: If the isAddNPCTarget variable is true for the enhancement from this iteration, it means,
		//[EN]: that applying this buff to an enemy NPC is intended.
		//[EN]: In this case, we add the buff to the NPC.
        for(int i = 0; i < level; i++)
            if(target.active && target.life > 0 && EffectsSystem.effectsInfo[i].isAddNPCTarget)
                target.AddBuff(EffectsSystem.effectsInfo[i].buffID, 60 * 60);
        
		//[RU]: Вызываем метод AddExp, передавая в него те параметры, которые должны быть переданы, в зависимости от выбранной категории основного предмета
		//[RU]: Данный метод является локальным, и определен ниже данных проверок.
		//[RU]: Подробное описание метода и принимаемых параметров смотрите в описании данного метода
		//------------------------------------------
		//[EN]: Call the AddExp method, passing the parameters that should be passed depending on the selected category of the main item.
		//[EN]: This method is local and defined below these checks.
		//[EN]: See the detailed description of the method and its parameters in the description of this method.
        if(LevelType == Common.PlayerSave.LevelType.Damage)
            AddExp(ref ps.PlayerExpDamage, 
            ref ps.NeedExpDamage, 
            ref ps.PlayerLevelDamage, 
            ps.MaxLevelDamage,
            Common.PlayerSave.StartNeedExpDamage,
            ps.GeometyProgressionDamage);
        else if(LevelType == Common.PlayerSave.LevelType.Speed)
            AddExp(ref ps.PlayerExpSpeed, 
            ref ps.NeedExpSpeed, 
            ref ps.PlayerLevelSpeed, 
            ps.MaxLevelSpeed,
            Common.PlayerSave.StartNeedExpSpeed,
            ps.GeometyProgressionSpeed);
        else if(LevelType == Common.PlayerSave.LevelType.Crit)
            AddExp(ref ps.PlayerExpCrit, 
            ref ps.NeedExpCrit, 
            ref ps.PlayerLevelCrit, 
            ps.MaxLevelCrit,
            Common.PlayerSave.StartNeedExpCrit,
            ps.GeometyProgressionCrit);
        else if(LevelType == Common.PlayerSave.LevelType.Mana)
            AddExp(ref ps.PlayerExpMana, 
            ref ps.NeedExpMana, 
            ref ps.PlayerLevelMana, 
            ps.MaxLevelMana,
            Common.PlayerSave.StartNeedExpMana,
            ps.GeometyProgressionMana);
        else if(LevelType == Common.PlayerSave.LevelType.Penetrate)
            AddExp(ref ps.PlayerExpPenetrate, 
            ref ps.NeedExpPenetrate, 
            ref ps.PlayerLevelPenetrate, 
            ps.MaxLevelPenetrate,
            Common.PlayerSave.StartNeedExpPenetrate,
            ps.GeometyProgressionPenetrate);
        else if(LevelType == Common.PlayerSave.LevelType.Effects)
            AddExp(ref ps.PlayerExpEffect, 
            ref ps.NeedExpEffect, 
            ref ps.PlayerLevelEffect, 
            EffectsSystem.MaxLevel + 1,
            Common.PlayerSave.StartNeedExpEffect,
            ps.GeometyProgressionEffect);

		//[RU]: Метод AddExp добавляем опыт для категории из переданных параметров
		//[RU]: Предпологается, что все передаваемые параметры будут относится к одной категории усилений
		//[RU]: exp - количество нанесенного урона персонажем для данной категории усилений
		//[RU]: needExp - необходимое количество нанесенного урона для перехода на следующий уровень
		//[RU]: level - текущий уровень игрока для данной категории
		//[RU]: maxLevel - максимальный уровень игрока для данной категории
		//[RU]: startNeedExp - количество нанесенного урона, необходимое для перехода с 0 на 1 уровень для данной категории
		//[RU]: geometyProgression - геометрическая прогрессия требуемого урона для данной категории
		//------------------------------------------
		//[EN]: The AddExp method adds experience for the category from the passed parameters.
		//[EN]: It is assumed that all passed parameters will relate to one category of enhancements.
		//[EN]: exp - the amount of damage dealt by the character for this category of enhancements
		//[EN]: needExp - the required amount of damage dealt to move to the next level
		//[EN]: level - the current level of the player for this category
		//[EN]: maxLevel - the maximum level of the player for this category
		//[EN]: startNeedExp - the amount of damage dealt required to move from 0 to 1 level for this category
		//[EN]: geometricProgression - geometric progression of the required damage for this category
        void AddExp(ref int exp, ref int needExp, ref int level, int maxLevel, int startNeedExp, float geometyProgression)
        {
			//[RU]: Добавляем к суммарному количество нанесенного урона данной категории нанесенный урон данному NPC
			//------------------------------------------
			//[EN]: Add the dealt damage to this NPC to the total damage dealt for this category.
            exp += damageDone;
			
			//[RU]: Проверяем, если количество нанесенного урона больше, чем максимальное количество урона необходимое для повышения уровня, и..
			//[RU]: Уровень персонажа в данной категории меньше, чем максимальный уровень
			//------------------------------------------
			//[EN]: Check if the amount of damage dealt is greater than the maximum amount of damage required to level up, and..
			//[EN]: The character's level in this category is less than the maximum level.
            if(exp >= needExp && level < maxLevel)
            {
				//[RU]: Уменьшаем количество нанесенного урона на необходимое количество урона для повышения уровня
				//------------------------------------------
				//[EN]: Reduce the amount of dealt damage by the required amount of damage to level up.
				exp = exp - needExp;
				
				//[RU]: Увеличиваем уровень персонажа в данной категории
				//------------------------------------------
				//[EN]: Increase the character's level in this category.
				level++;
				
				//[RU]: RecalculationNeedPlayerExp расчитывает количество опыта необходимое для перехода на следующий уровень для каждой категории.
				//[RU]: См. Common/Player/PlayerSave.cs
				//------------------------------------------
				//[EN]: The RecalculationNeedPlayerExp method calculates the amount of experience required to advance to the next level for each category.
				//[EN]: See Common/Player/PlayerSave.cs
				needExp = Common.PlayerSave.RecalculationNeedPlayerExp(level, startNeedExp, geometyProgression);
            }
			//[RU]: Иначе, если уровень игрока больше или равен максимальному уровню данной категори...
			//[RU]: Значение количества нанесенного урона делаем равным значению количество нанесенного урона для повышения уровня
			//------------------------------------------
			//[EN]: Otherwise, if the player's level is greater than or equal to the maximum level for this category...
			//[EN]: Set the amount of dealt damage to the value of the amount of damage required to level up.
			else if(level >= maxLevel)
				exp = needExp;
        }
    }

	//[RU]: Метод OnSpawn вызывается 1 раз при появлении снаряда в игровом мире
	//------------------------------------------
	//[EN]: The OnSpawn method is called once when the projectile appears in the game world.
    public override void OnSpawn(IEntitySource source)
    {
		//[RU]: Если source можно привести к типу EntitySource_ItemUse
		//------------------------------------------
		//[EN]: If source can be cast to the type EntitySource_ItemUse
        if(source is EntitySource_ItemUse itemUse)
        {
            Item item = itemUse.Item;
			
			//[RU]: Если тип предмета, который заспавнил данный снаряд, равен основному предмету мода
			//------------------------------------------
			//[EN]: If the type of the item that spawned this projectile is equal to the main item of the mod.
            if(item.type == ModContent.ItemType<Items.RazorbladeTyphoonProgress>())
            {
				//[RU]: Присваиваем переменной LevelType значение текущей выбранной категории на освном предмете
				//------------------------------------------
				//[EN]: Assign the LevelType variable the value of the currently selected category on the main item.
                Items.RazorbladeTyphoonProgress razorbladeTyphoonProgress = (Items.RazorbladeTyphoonProgress)item.ModItem;
                LevelType = (Common.PlayerSave.LevelType)razorbladeTyphoonProgress.Page;
            }
        }
    }
}