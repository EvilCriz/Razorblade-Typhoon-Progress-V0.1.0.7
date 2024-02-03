using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Localization;
using System.Collections.Generic;
using Terraria;

using LevelType = RazorbladeTyphoonProgress.Common.PlayerSave.LevelType;
using Microsoft.Xna.Framework;

namespace RazorbladeTyphoonProgress.Items;

//[RU]: Класс для основного предмета мода. (Аналог Бритвенного тайфуна)
//------------------------------------------
//[EN]: Class for the main item of the mod. (Analogous to the Razor Typhoon)
public class RazorbladeTyphoonProgress : ModItem
{
	//[RU]: Переопределение расположения текстуры данного предмета в проекте
	//------------------------------------------
	//[EN]: Override the location of the texture for this item in the project.
	public override string Texture => "RazorbladeTyphoonProgress/Content/Items/" + nameof(RazorbladeTyphoonProgress);
	
	//[RU]: Данная строка указывает, что локализованный текст по ключу "Tooltip" не должен создаваться в файлах локализации
	//------------------------------------------
	//[EN]: This line indicates that the localized text with the key "Tooltip" should not be created in the localization files.
    public override LocalizedText Tooltip => LocalizedText.Empty;
	
	//[RU]: Ключ для регистрации локализованного текста в файлах локализации.
	//[RU]: Используется для регистрации локализованного текста в коде ниже
	//------------------------------------------
	//[EN]: Key for registering localized text in localization files.
	//[EN]: Used for registering localized text in the code below.
	private const string LocalizedKey = "Mods.RazorbladeTyphoonProgress.Items.RazorbladeTyphoonProgress.";
	
	//[RU]: Заголовок для категории "Урон".
	//------------------------------------------
	//[EN]: Header for the "Damage" category.
    LocalizedText TitleDamageInfo = Language.GetOrRegister(LocalizedKey + nameof(TitleDamageInfo));
	
	//[RU]: Заголовок для категории "Скорость атаки".
	//------------------------------------------
	//[EN]: Header for the "Attack Speed" category.
    LocalizedText TitleSpeedInfo = Language.GetOrRegister(LocalizedKey + nameof(TitleSpeedInfo));
	
	//[RU]: Заголовок для категории "Шанс критической атаки".
	//------------------------------------------
	//[EN]: Header for the "Critical Strike Chance" category.
    LocalizedText TitleCritInfo = Language.GetOrRegister(LocalizedKey + nameof(TitleCritInfo));
	
	//[RU]: Заголовок для категории "Стоимость маны".
	//------------------------------------------
	//[EN]: Header for the "Mana Cost" category.
    LocalizedText TitleManaInfo = Language.GetOrRegister(LocalizedKey + nameof(TitleManaInfo));
	
	//[RU]: Заголовок для категории "Дополнительное количество атак снаряда".
	//------------------------------------------
	//[EN]: Header for the "Additional Projectile Attacks" category.
    LocalizedText TitlePenetrateInfo = Language.GetOrRegister(LocalizedKey + nameof(TitlePenetrateInfo));
	
	//[RU]: Заголовок для категории "Дополнительные эффекты".
	//------------------------------------------
	//[EN]: Header for the "Additional Effects" category.
    LocalizedText TitleEffectsInfo = Language.GetOrRegister(LocalizedKey + nameof(TitleEffectsInfo));
	
	//[RU]: Подсказка для переключения страниц описания предмета (категорий)
	//------------------------------------------
	//[EN]: Tooltip for switching item description pages (categories)
    LocalizedText SwapPageTooltip = Language.GetOrRegister(LocalizedKey + nameof(SwapPageTooltip));
	
	//[RU]: Информации о текущем прогрессе (уровень, макс. уровень, нанесенный урон, требуемое количество урона)
	//------------------------------------------
	//[EN]: Information about the current progress (level, max level, damage dealt, required amount of damage).
    LocalizedText LevelInfo = Language.GetOrRegister(LocalizedKey + nameof(LevelInfo));
	
	//[RU]: Описание баффа игрока (из категории "Дополнительные эффекты")
	//------------------------------------------
	//[EN]: Player buff description (from the "Additional Effects" category).
    LocalizedText EffectsInfo = Language.GetOrRegister(LocalizedKey + nameof(EffectsInfo));
	
	//[RU]: Описание баффа NPC (из категории "Дополнительные эффекты")
	//------------------------------------------
	//[EN]: NPC buff description (from the "Additional Effects" category).
    LocalizedText EffectsNPCInfo = Language.GetOrRegister(LocalizedKey + nameof(EffectsNPCInfo));
	
	//[RU]: Описание баффа игрока, которые должны активироваться горячей клавишей (из категории "Дополнительные эффекты")
	//[RU]: (В данный момент подобные баффы не реализованы)
	//------------------------------------------
	//[EN]: Player buff description, which should be activated by a hotkey (from the "Additional Effects" category).
	//[EN]: (Currently, such buffs are not implemented)
    LocalizedText EffectsIsUseKeyInfo = Language.GetOrRegister(LocalizedKey + nameof(EffectsIsUseKeyInfo));
	
	//[RU]: Описание баффа "Снаряды проходят сквозь блоки" (из категории "Дополнительные эффекты")
	//------------------------------------------
	//[EN]: Description of the "Projectiles pass through blocks" buff (from the "Additional Effects" category).
    LocalizedText EffectsIsNoCollideInfo = Language.GetOrRegister(LocalizedKey + nameof(EffectsIsNoCollideInfo));
	
	//[RU]: Локализованный текст "Открытые"
	//------------------------------------------
	//[EN]: Localized text "Open"
    LocalizedText Open = Language.GetOrRegister(LocalizedKey + nameof(Open));
	
	//[RU]: Локализованный текст "Закрытые"
	//------------------------------------------
	//[EN]: Localized text "Closed"
    LocalizedText Close = Language.GetOrRegister(LocalizedKey + nameof(Close));
	
	//[RU]: Локализованный текст "Максимум"
	//------------------------------------------
	//[EN]: Localized text "Maximum"
    LocalizedText Maximum = Language.GetOrRegister(LocalizedKey + nameof(Maximum));
	
	//[RU]: Текущая страница описания (категория)
	//------------------------------------------
	//[EN]: Current description page (category).
    public int Page = 1;
	
	//[RU]: Максимальное количество страниц описания (категорий)
	//------------------------------------------
	//[EN]: Maximum number of description pages (categories).
    public int MaxPage = 6;
	
	//[RU]: Вспомогательный класс "EffectsSystem" для категории "Дополнительные эффекты"
	//[RU]: См. CustomClasses/EffectsSystem.cs
	//------------------------------------------
	//[EN]: Auxiliary class "EffectsSystem" for the "Additional Effects" category.
	//[EN]: See CustomClasses/EffectsSystem.cs
    public CustCl.EffectsSystem EffectsSystem = CustCl.EffectsSystem.CreateEffectSystem();
	
	//[RU]: Базовое количество затрачиваемой маны на использование предмета
	//------------------------------------------
	//[EN]: Base amount of mana consumed when using the item.
	public int BaseManaCost = 30;
	
    public override void SetDefaults()
    {
        Item.damage = 7;
        Item.DamageType = DamageClass.Magic;
		Item.width = 32;
		Item.height = 32;
        Item.useTime = 120;
        Item.useAnimation = 120;
        Item.useStyle = ItemUseStyleID.Shoot;
		Item.knockBack = 1;
		Item.value = 25;
		Item.rare = ItemRarityID.Red;
        Item.mana = BaseManaCost;
        Item.crit = 0;
        Item.UseSound = SoundID.Item84;
		
		//[RU]: Снаряд, который будет выпускать данный предмет при использовании.
		//[RU]: См. Content/Projectiles/ProjectileRazorbladeTyphoonProgress.cs
		//------------------------------------------
		//[EN]: Projectile that this item will release upon usage.
		//[EN]: See Content/Projectiles/ProjectileRazorbladeTyphoonProgress.cs
        Item.shoot = ModContent.ProjectileType<Projectiles.ProjectileRazorbladeTyphoonProgress>();
		
        Item.shootSpeed = 12f;
        Item.autoReuse = true;
    }
	
	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient(ItemID.FallenStar, 3);
		recipe.AddRecipeGroup(RecipeGroupID.IronBar, 6);
		recipe.AddTile(TileID.Anvils);
		recipe.Register();
	}
	
	//[RU]: Указаывается, что данный предмет сам не наносит урон.
	//[RU]: Если этого не делать, то при использовании предмета впритык к NPC - он получит урон
	//------------------------------------------
	//[EN]: Specify that this item does not deal damage itself.
	//[EN]: If not done, using the item directly against an NPC will cause damage.
	public override bool? CanHitNPC(Player player, NPC target) => false;
	
	//[RU]: Указываем, что предмет не должен расходоваться
	//[RU]: Если это не сделать, то при переключении страниц (категорий) на ПКМ в инвентаре - предмет будет сразу израсходован
	//------------------------------------------
	//[EN]: Specify that the item should not be consumed.
	//[EN]: If not done, switching pages (categories) with right-click in the inventory will immediately consume the item.
	public override bool ConsumeItem (Player player) => false;
	
	//[RU]: Указываем, что данный предмет можно использовать на ПКМ в инвентаре
	//------------------------------------------
	//[EN]: Specify that this item can be used with right-click in the inventory.
	public override bool CanRightClick() => true;
	
	//[RU]: Переключаем страницу при нажатии ПКМ по данному предмету в инвентаре
	//------------------------------------------
	//[EN]: Switch the page when right-clicking on this item in the inventory.
	public override void RightClick(Player player) => Page = Page >= MaxPage ? 1 : Page + 1;
	
	//[RU]: Добавляем урон данному предмету в зависимости от уровня персонажа в категории "Урон"
	//[RU]: damage.Base означает, что мы добавляем фиксированный урон ДО усилений (модификациями, зельями, аксессуарами и т.д.)
	//------------------------------------------
	//[EN]: Add damage to this item based on the character's level in the "Damage" category.
	//[EN]: damage.Base means that we are adding fixed damage BEFORE enhancements (modifications, potions, accessories, etc.).
    public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        => damage.Base += player.GetModPlayer<Common.PlayerSave>().PlayerLevelDamage;
		
	//[RU]: Добавляем крит. шанс данному предмету в зависимости от уровня персонажа в категории "Шанс критического удара"
	//------------------------------------------
	//[EN]: Add critical strike chance to this item based on the character's level in the "Critical Strike Chance" category.
    public override void ModifyWeaponCrit(Player player, ref float crit)
        => crit += player.GetModPlayer<Common.PlayerSave>().PlayerLevelCrit;
		
	//[RU]: Уменьшаем стоимость маны предмета в зависимости от уровня персонажа в категории "Стоимость маны"
	//------------------------------------------
	//[EN]: Decrease the mana cost of the item based on the character's level in the "Mana Cost" category.
    public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        => Item.mana = BaseManaCost - player.GetModPlayer<Common.PlayerSave>().PlayerLevelMana;
		
	//[RU]: Уменьшаем время использования предмета (т.е. увеличиваем скорость атаки)
	//[RU]: 1.0f - стандартная скорость, 1.2f - на 20% время использования предмета меньше
	//------------------------------------------
	//[EN]: Reducing item usage time (i.e., increasing attack speed)
	//[EN]: 1.0f - standard speed, 1.2f - 20% faster item usage time
    public override float UseSpeedMultiplier(Player player)
        => 1.0f + 0.2f * player.GetModPlayer<Common.PlayerSave>().PlayerLevelSpeed;
	
	//[RU]: Добавляем игроку баффы из категории "Дополнительные эффекты", если была затрачена мана на использование данного предмета
	//------------------------------------------
	//[EN]: Granting buffs to the player from the "Additional Effects" category if mana was spent on using this item
    public override void OnConsumeMana(Player player, int manaConsumed)
    {
		//[RU]: Получаем экземпляр класса PlayerSave игрока, который потрил ману на использование данного предмета.
		//[RU]: Класс PlayerSave содержит все данные о текущем уровне, нанесенном уроне и т.д. для каждой категории
		//[RU]: Исключение: максимальный уровень категории "Дополнительные эффекты" хранится в классе "EffectsSystem"
		//[RU]: См. Common/Player/PlayerSave.cs
		//------------------------------------------
		//[EN]: Obtaining an instance of the PlayerSave class for a player who spent mana on using this item.
		//[EN]: The PlayerSave class contains all data about the current level, inflicted damage, etc., for each category.
		//[EN]: Exception: the maximum level of the "Additional Effects" category is stored in the "EffectsSystem" class.
		//[EN]: See Common/Player/PlayerSave.cs
        Common.PlayerSave playerSave = player.GetModPlayer<Common.PlayerSave>();
		
		//[RU]: Получаем уровень игрока в категории "Дополнительные эффекты"
		//[RU]: Если он превышает максимальный уровень категории "Дополнительные эффекты" - возвращаем максимальный уровень данной категории.
		//[RU]: Сделано для предотвращения ошибок, в случае уменьшения максимального уровня данной категории в будущем
		//------------------------------------------
		//[EN]: Retrieving the player's level in the "Additional Effects" category.
		//[EN]: If it exceeds the maximum level of the "Additional Effects" category, return the maximum level for this category.
		//[EN]: Done to prevent errors in case the maximum level for this category is reduced in the future.
        int level = playerSave.PlayerLevelEffect <= EffectsSystem.MaxLevel + 1 ? 
            playerSave.PlayerLevelEffect : EffectsSystem.MaxLevel + 1;
        
		//[RU]: Перебираем все эффекты категории "Дополнительные эффекты" до (включительно) текущего уровня персонажа данной категории
		//[RU]: Пропускаем перебираемый эффект, если  "isAddNPCTarget", "isUsePerKey" или "isNoCollide" были истинны
		//[RU]: Описание данных переменных см. в классе CustomClasses/EffectsSystem.cs
		//------------------------------------------
		//[EN]: Iterating through all effects in the "Additional Effects" category up to (including) the current level of the character in this category.
		//[EN]: Skipping the iterated effect if "isAddNPCTarget," "isUsePerKey," or "isNoCollide" were true.
		//[EN]: See the class CustomClasses/EffectsSystem.cs for the description of these variables.
        for(int i = 0; i < level; i++)
            if(!EffectsSystem.effectsInfo[i].isAddNPCTarget && 
                !EffectsSystem.effectsInfo[i].isUsePerKey && 
                !EffectsSystem.effectsInfo[i].isNoCollide)
                    player.AddBuff(EffectsSystem.effectsInfo[i].buffID, 60 * 60);
    }
	
    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
		//[RU]: Получаем экземпляр класса PlayerSave локального игрока.
		//[RU]: Класс PlayerSave содержит все данные о текущем уровне, нанесенном уроне и т.д. для каждой категории
		//[RU]: Исключение: максимальные уровень категории "Дополнительные эффекты" хранится в классе "EffectsSystem"
		//[RU]: См. Common/Player/PlayerSave.cs
		//------------------------------------------
		//[EN]: Obtaining an instance of the PlayerSave class for the local player.
		//[EN]: The PlayerSave class contains all data about the current level, inflicted damage, etc., for each category.
		//[EN]: Exception: the maximum level of the "Additional Effects" category is stored in the "EffectsSystem" class.
		//[EN]: See Common/Player/PlayerSave.cs
        Player player = Main.LocalPlayer;
        Common.PlayerSave playerSave = player.GetModPlayer<Common.PlayerSave>();
		
		//[RU]: Перечисление, хранящее в себе список всех категорий улучшений
		//[RU]: Определен в классе Common/Player/PlayerSave.cs
		//------------------------------------------
		//[EN]: Enumeration containing a list of all improvement categories.
		//[EN]: Defined in the class Common/Player/PlayerSave.cs
        LevelType levelType = (LevelType)Page;

		//[RU]: Получаем заголовок для выбранной (с помощью ПКМ в инвентаре) категории.
		//------------------------------------------
		//[EN]: Obtaining the header for the selected (using right-click in the inventory) category.
        string titleInfo = levelType switch
        {
            LevelType.Damage => TitleDamageInfo.Value,
            LevelType.Speed => TitleSpeedInfo.Value,
            LevelType.Crit => TitleCritInfo.Value,
            LevelType.Mana => TitleManaInfo.Value,
            LevelType.Penetrate => TitlePenetrateInfo.Value,
            LevelType.Effects => TitleEffectsInfo.Value,
            _ => ""
        };

		//[RU]: Выводим заголовок для выбранной (с помощью ПКМ в инвентаре) категории.
		//[RU]: tooltips.Insert(1, line) - указывает на то, что заголовок должен выводиться сразу после названия предмета
		//------------------------------------------
		//[EN]: Displaying the header for the selected (using right-click in the inventory) category.
		//[EN]: tooltips.Insert(1, line) - indicates that the header should be displayed right after the item name.
        var line = new TooltipLine(Mod, "", titleInfo);
        line.OverrideColor = Color.RoyalBlue;
        tooltips.Insert(1, line);

		//[RU]: Переменные для хранения информации о текущем прогрессе для выбранной (с помощью ПКМ в инвентаре) категории.
		//------------------------------------------
		//[EN]: Variables to store information about the current progress for the selected (using right-click in the inventory) category.
        int Level = 0;
        int Exp = 0;
        int NeedExp = 0;
        int MaxLevel = 0;

		//[RU]: Заполняем эти переменные нужными значениями в зависимости от выбранной (с помощью ПКМ в инвентаре) категории.
		//------------------------------------------
		//[EN]: Populating these variables with the necessary values depending on the selected (using right-click in the inventory) category.
        if(Page == (int)LevelType.Damage)
        {
            Level = playerSave.PlayerLevelDamage;
            Exp = playerSave.PlayerExpDamage;
            NeedExp = playerSave.NeedExpDamage;
            MaxLevel = playerSave.MaxLevelDamage;
        }
        else if(Page == (int)LevelType.Speed)
        {
            Level = playerSave.PlayerLevelSpeed;
            Exp = playerSave.PlayerExpSpeed;
            NeedExp = playerSave.NeedExpSpeed;
            MaxLevel = playerSave.MaxLevelSpeed;
        }
        else if(Page == (int)LevelType.Crit)
        {
            Level = playerSave.PlayerLevelCrit;
            Exp = playerSave.PlayerExpCrit;
            NeedExp = playerSave.NeedExpCrit;
            MaxLevel = playerSave.MaxLevelCrit;
        }
        else if(Page == (int)LevelType.Mana)
        {
            Level = playerSave.PlayerLevelMana;
            Exp = playerSave.PlayerExpMana;
            NeedExp = playerSave.NeedExpMana;
            MaxLevel = playerSave.MaxLevelMana;
        }
        else if(Page == (int)LevelType.Penetrate)
        {
            Level = playerSave.PlayerLevelPenetrate;
            Exp = playerSave.PlayerExpPenetrate;
            NeedExp = playerSave.NeedExpPenetrate;
            MaxLevel = playerSave.MaxLevelPenetrate;
        }
        else if(Page == (int)LevelType.Effects)
        {
            Level = playerSave.PlayerLevelEffect;
            Exp = playerSave.PlayerExpEffect;
            NeedExp = playerSave.NeedExpEffect;
            MaxLevel = EffectsSystem.MaxLevel + 1;
        }

		//[RU]: Выводим информации о текущем прогрессе для выбранной (с помощью ПКМ в инвентаре) категории.
		//[RU]: tooltips.Insert(2, line) - указывает на то, что информация должна выводиться сразу после заголовка категории
		//------------------------------------------
		//[EN]: Displaying information about the current progress for the selected (using right-click in the inventory) category.
		//[EN]: tooltips.Insert(2, line) - indicates that the information should be displayed right after the category header.
        string levelInfoFormat = LevelInfo.Format(Level, MaxLevel, Exp, NeedExp);
        line = new TooltipLine(Mod, "", levelInfoFormat);
        line.OverrideColor = Color.Yellow;
        tooltips.Insert(2, line);

		//[RU]: Вывод локализованного текста "Максимум" если уровень персонажа для данной категории больше или равен макс. уровню данной категори.
		//[RU]: tooltips.Insert(3, line) - указывает на то, что информация должна выводиться сразу после вывода информации о текущем прогрессе.
		//------------------------------------------
		//[EN]: Displaying the localized text "Maximum" if the character's level for this category is greater than or equal to the max level for this category.
		//[EN]: tooltips.Insert(3, line) - indicates that the information should be displayed right after displaying information about the current progress.
        if(Level >= MaxLevel)
        {
            line = new TooltipLine(Mod, "", Maximum.Value);
            line.OverrideColor = Color.Yellow;
            tooltips.Insert(3, line);
        }

		//[RU]: Дополнительный вывод информации, если выбрана категори "Дополнительные эффекты"
		//------------------------------------------
		//[EN]: Additional information display if the "Additional Effects" category is selected.
        if(Page == (int)LevelType.Effects)
        {
			//[RU]: Список, для хранения всех усилений категори "Дополнительные эффекты"
			//[RU]: Локализованный текст получается в зависимости от значений полей класса "EffectsSystem":
			//[RU]: "buffID", "isAddNPCTarget", "isUsePerKey", "isNoCollide"
			//[RU]: Описание данных полей смотрите в классе CustomClasses/EffectsSystem.cs
			//------------------------------------------
			//[EN]: List for storing all enhancements of the "Additional Effects" category.
			//[EN]: Localized text is obtained based on the values of the fields in the "EffectsSystem" class:
			//[EN]: "buffID," "isAddNPCTarget," "isUsePerKey," "isNoCollide."
			//[EN]: See the class CustomClasses/EffectsSystem.cs for the description of these fields.
            List<string> BuffLangList = new();
            for(int i = 0; i < EffectsSystem.effectsInfo.Count; i++)
            {
                int buffID = EffectsSystem.effectsInfo[i].buffID;
                string langBuff = "";
				if(buffID != -1)
					langBuff = Lang.GetBuffName(buffID);
                string effectsInfo;
                if(EffectsSystem.effectsInfo[i].isAddNPCTarget)
                    effectsInfo = EffectsNPCInfo.Format(langBuff);
                else if(EffectsSystem.effectsInfo[i].isUsePerKey)
                    effectsInfo = EffectsIsUseKeyInfo.Format(langBuff);
                else if(EffectsSystem.effectsInfo[i].isNoCollide)
                    effectsInfo = EffectsIsNoCollideInfo.Value;
                else
                    effectsInfo = EffectsInfo.Format(langBuff);
                BuffLangList.Add(effectsInfo);
            }
			
			//[RU]: Если уровень персонажа в категории "Дополнительные эффекты" больше 0:
			//[RU]: Вывод локализованного текста "Открытые"
			//------------------------------------------
			//[EN]: If the character's level in the "Additional Effects" category is greater than 0:
			//[EN]: Displaying the localized text "Unlocked."
			if(Level > 0)
			{
				line = new TooltipLine(Mod, "", Open.Value);
				line.OverrideColor = Color.LimeGreen;
				tooltips.Add(line);
			}
			
			//[RU]: Вывод описания всех открытых улучшений для персонажа категории "Дополнительные эффекты"
			//------------------------------------------
			//[EN]: Displaying the description of all unlocked enhancements for the character in the "Additional Effects" category.
            for(int i = 0; i < Level; i++)
            {
                line = new TooltipLine(Mod, "", BuffLangList[i]);
                line.OverrideColor = Color.RoyalBlue;
                tooltips.Add(line);
            }
			
			//[RU]: Если уровень персонажа в категории "Дополнительные эффекты" меньше максимального:
			//[RU]: Вывод локализованного текста "Закрытые"
			//------------------------------------------
			//[EN]: If the character's level in the "Additional Effects" category is less than the maximum:
			//[EN]: Displaying the localized text "Locked."
			if(Level < MaxLevel)
			{
				line = new TooltipLine(Mod, "", Close.Value);
				line.OverrideColor = Color.DarkViolet;
				tooltips.Add(line);
			}
			
			//[RU]: Вывод описания всех закрытых улучшений для персонажа категории "Дополнительные эффекты"
			//------------------------------------------
			//[EN]: Displaying the description of all locked enhancements for the character in the "Additional Effects" category.
			for(int i = Level; i < MaxLevel; i++)
            {
                line = new TooltipLine(Mod, "", BuffLangList[i]);
                line.OverrideColor = Color.DarkRed;
                tooltips.Add(line);
            }
        }

		//[RU]: Вывод информации о текущей странице, максимальном количестве страниц и подсказки о переключении страниц.
		//------------------------------------------
		//[EN]: Displaying information about the current page, the maximum number of pages, and hints for page navigation.
        string swapPageTooltipFormat = SwapPageTooltip.Format(Page, MaxPage);
        line = new TooltipLine(Mod, "", swapPageTooltipFormat);
        line.OverrideColor = Color.Yellow;
        tooltips.Add(line);
    }
}