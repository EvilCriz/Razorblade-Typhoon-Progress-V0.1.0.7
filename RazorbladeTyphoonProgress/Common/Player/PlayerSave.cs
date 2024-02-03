using System;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace RazorbladeTyphoonProgress.Common;

//[RU]: Класс PlayerSave содержит все данные о текущем уровне, нанесенном уроне и т.д. для каждой категории
//[RU]: Исключение: максимальные уровень категории "Дополнительные эффекты" хранится в классе "EffectsSystem"
//------------------------------------------
//[EN]: The PlayerSave class contains all data about the current level, damage dealt, etc. for each category.
//[EN]: Exception: The maximum level of the "Additional Effects" category is stored in the "EffectsSystem" class.
public class PlayerSave : ModPlayer
{
	//[RU]: Перечисление, хранящее в себе список всех категорий улучшений
	//[RU]: Определен в классе Common/Player/PlayerSave.cs
	//------------------------------------------
	//[EN]: Enumeration containing a list of all improvement categories
	//[EN]: Defined in the class Common/Player/PlayerSave.cs
    public enum LevelType 
    {
        Nan = 0,
        Damage,
        Speed,
        Crit,
        Mana,
        Penetrate,
        Effects
    }

	//[RU]: Количество нанесеного урона, которое необходимо для перехода с 0 на 1 уровень для каждой категори
	//[RU]: Не сохраняется в файлах персонажа
	//------------------------------------------
	//[EN]: The amount of damage dealt required to move from 0 to 1 level for each category
	//[EN]: Not saved in character files.
    public const int StartNeedExpDamage = 1000;
    public const int StartNeedExpSpeed = 400;
    public const int StartNeedExpCrit = 4000;
    public const int StartNeedExpMana = 300;
    public const int StartNeedExpPenetrate = 300;
    public const int StartNeedExpEffect = 1500;
	
    //[RU]: Переменные, хранящие в себе необходимое количество нанесенного урона для перехода на следующий уровень для каждой категории
	//[RU]: По умолчанию имеет значение, необходимое для перехода с 0 на 1 уровень.
	//[RU]: Не сохраняется в файлах персонажа
	//------------------------------------------
	//[EN]: Variables storing the required amount of damage dealt to move to the next level for each category
	//[EN]: By default, they have the value required to move from 0 to 1 level.
	//[EN]: Not saved in character files.
    public int NeedExpDamage = StartNeedExpDamage;
    public int NeedExpSpeed = StartNeedExpSpeed;
    public int NeedExpCrit = StartNeedExpCrit;
    public int NeedExpMana = StartNeedExpMana;
    public int NeedExpPenetrate = StartNeedExpPenetrate;
    public int NeedExpEffect = StartNeedExpEffect;
    
	//[RU]: Нанесенный игроком урон на текущем уровне для каждой категории
	//[RU]: Сохраняется в файлах персонажа (см. ниже)
	//------------------------------------------
	//[EN]: The damage dealt by the player at the current level for each category
	//[EN]: Saved in character files (see below)
    public int PlayerExpDamage = 0;
    public int PlayerExpSpeed = 0;
    public int PlayerExpCrit = 0;
    public int PlayerExpMana = 0;
    public int PlayerExpPenetrate = 0;
    public int PlayerExpEffect = 0;
    
	//[RU]: Текущий уровень персонажа для каждой категории
	//[RU]: Сохраняется в файлах персонажа (см. ниже)
	//------------------------------------------
	//[EN]: The current level of the character for each category
	//[EN]: Saved in character files (see below)
    public int PlayerLevelDamage = 0;
    public int PlayerLevelSpeed = 0;
    public int PlayerLevelCrit = 0;
    public int PlayerLevelMana = 0;
    public int PlayerLevelPenetrate = 0;
    public int PlayerLevelEffect = 0;
    
	//[RU]: Геометрическая прогрессия увеличения количество требуемого опыта для перехода на новый уровень для каждой категории
	//[RU]: Не сохраняется в файлах персонажа
	//------------------------------------------
	//[EN]: Geometric progression of increasing the amount of required experience to move to a new level for each category
	//[EN]: Not saved in character files.
    public float GeometyProgressionDamage = 1.15f;
    public float GeometyProgressionSpeed = 1.15f;
    public float GeometyProgressionCrit = 1.15f;
    public float GeometyProgressionMana = 1.30f;
    public float GeometyProgressionPenetrate = 2.0f;
    public float GeometyProgressionEffect = 1.60f;
    
	//[RU]: Максимальный уровень для каждой категории
	//[RU]: Исключение: максимальный уровень категории "Дополнительные эффекты" хранится в классе "EffectsSystem"
	//[RU]: Не сохраняется в файлах персонажа
	//------------------------------------------
	//[EN]: Maximum level for each category
	//[EN]: Exception: The maximum level of the "Additional Effects" category is stored in the "EffectsSystem" class.
	//[EN]: Not saved in character files.
    public int MaxLevelDamage = 9999;
    public int MaxLevelSpeed = 45;
    public int MaxLevelCrit = 46;
    public int MaxLevelMana = 20;
    public int MaxLevelPenetrate = 25;
	
	//[RU]: Метод, вызывающийся каждый раз, при сохранении персонажа
	//------------------------------------------
	//[EN]: Method called every time the character is saved.
    public override void SaveData(TagCompound tag) 
	{
		//[RU]: Сохраняем значение переменных в tag, где в качестве ключа используется имя данной переменной.
		//------------------------------------------
		//[EN]: Save the values of variables in the tag, where the variable name is used as the key.
        tag[nameof(PlayerExpDamage)] = PlayerExpDamage;
        tag[nameof(PlayerExpSpeed)] = PlayerExpSpeed;
        tag[nameof(PlayerExpCrit)] = PlayerExpCrit;
        tag[nameof(PlayerExpMana)] = PlayerExpMana;
        tag[nameof(PlayerExpPenetrate)] = PlayerExpPenetrate;
        tag[nameof(PlayerExpEffect)] = PlayerExpEffect;
        //
        tag[nameof(PlayerLevelDamage)] = PlayerLevelDamage;
        tag[nameof(PlayerLevelSpeed)] = PlayerLevelSpeed;
        tag[nameof(PlayerLevelCrit)] = PlayerLevelCrit;
        tag[nameof(PlayerLevelMana)] = PlayerLevelMana;
        tag[nameof(PlayerLevelPenetrate)] = PlayerLevelPenetrate;
        tag[nameof(PlayerLevelEffect)] = PlayerLevelEffect;
    }

	//[RU]: Метод, вызывающийся при загрузке всех персонажей (вызывается отдельно для каждого персонажа)
	//------------------------------------------
	//[EN]: Method called when loading all characters (called separately for each character)
    public override void LoadData(TagCompound tag)
	{
		//[RU]: Загружаем значение переменных в tag, где в качестве ключа используется имя данной переменной.
		//------------------------------------------
		//[EN]: Load the values of variables from the tag, where the variable name is used as the key.
        PlayerExpDamage = tag.GetInt(nameof(PlayerExpDamage));
        PlayerExpSpeed = tag.GetInt(nameof(PlayerExpSpeed));
        PlayerExpCrit = tag.GetInt(nameof(PlayerExpCrit));
        PlayerExpMana = tag.GetInt(nameof(PlayerExpMana));
        PlayerExpPenetrate = tag.GetInt(nameof(PlayerExpPenetrate));
        PlayerExpEffect = tag.GetInt(nameof(PlayerExpEffect));
        //
        PlayerLevelDamage = tag.GetInt(nameof(PlayerLevelDamage));
        PlayerLevelSpeed = tag.GetInt(nameof(PlayerLevelSpeed));
        PlayerLevelCrit = tag.GetInt(nameof(PlayerLevelCrit));
        PlayerLevelMana = tag.GetInt(nameof(PlayerLevelMana));
        PlayerLevelPenetrate = tag.GetInt(nameof(PlayerLevelPenetrate));
        PlayerLevelEffect = tag.GetInt(nameof(PlayerLevelEffect));
        
		//[RU]: Расчитывает количество опыта необходимое для перехода на следующий уровень для каждой категории.
		//[RU]: См. подробное описание данного метода ниже
		//------------------------------------------
		//[EN]: Calculate the amount of experience required to move to the next level for each category.
		//[EN]: See the detailed description of this method below.
        NeedExpDamage = RecalculationNeedPlayerExp(PlayerLevelDamage, StartNeedExpDamage, GeometyProgressionDamage);
        NeedExpSpeed = RecalculationNeedPlayerExp(PlayerLevelSpeed, StartNeedExpSpeed, GeometyProgressionSpeed);
        NeedExpCrit = RecalculationNeedPlayerExp(PlayerLevelCrit, StartNeedExpCrit, GeometyProgressionCrit);
        NeedExpMana = RecalculationNeedPlayerExp(PlayerLevelMana, StartNeedExpMana, GeometyProgressionMana);
        NeedExpPenetrate = RecalculationNeedPlayerExp(PlayerLevelPenetrate, StartNeedExpPenetrate, GeometyProgressionPenetrate);
        NeedExpEffect = RecalculationNeedPlayerExp(PlayerLevelEffect, StartNeedExpEffect, GeometyProgressionEffect);
    }

	//[RU]: Расчитывает количество опыта необходимое для перехода на следующий уровень для каждой категории.
	//[RU]: level - подразумевается, что будет передан текущий уровень персонажа в определенной категории
	//[RU]: startNeedExp - подразумевается, что будет передано кол-во урона, которое требуется для перехода с 0 на 1 уровень в определенной категории
	//[RU]: geometyProgression - подразумевается, что будет передано значение геометрической прогрессии для определенной категории
	//[RU]: Возвращает количество требуемого урона, необходимое для перехода на следующий уровень для определенной категории
	//------------------------------------------
	//[EN]: Calculates the amount of experience needed to advance to the next level for each category.
	//[EN]: level - it is assumed that the current level of the character in a specific category will be passed.
	//[EN]: startNeedExp - it is assumed that the amount of damage required to transition from level 0 to level 1 in a specific category will be passed.
	//[EN]: geometricProgression - it is assumed that the value of the geometric progression for a specific category will be passed.
	//[EN]: Returns the amount of damage required to advance to the next level for a specific category.
    public static int RecalculationNeedPlayerExp(int level, int startNeedExp, float geometyProgression)
    {
		//[RU]: Присваиваем значение переменной result - значение, необходимого кол-во урона, для перехода с 0 на 1 уровень
		//[RU]: Если переданный уровень игрока меньше 1, то это значение и будет возвращено
		//------------------------------------------
		//[EN]: Assigning the value to the variable 'result' - the value of the required amount of damage to transition from level 0 to level 1.
		//[EN]: If the passed player level is less than 1, this value will be returned.
        int result = startNeedExp;
		
		//[RU]: Максимальное количество нанесенного урона, которое требуется для перехода на следующий уровень
		//------------------------------------------
		//[EN]: The maximum amount of damage dealt required to advance to the next level.
		int maxNeedExp = 10000000;
		
		//[RU]: Перебираем цикл от 0 до текущего уровня игрока данной категории
		//------------------------------------------
		//[EN]: Iterating the loop from 0 to the current level of the player in this category.
        for(int i = 0; i < level; i++)
		{
			//[RU]: Умножаем значение result в текущей итерации на значение геометрической прогрессии.
			//[RU]: Округляем до большего значения
			//------------------------------------------
			//[EN]: Multiply the value of 'result' in the current iteration by the value of the geometric progression.
			//[EN]: Round up to the nearest integer value.
            result = (int)Math.Ceiling((float)(result * geometyProgression));
			
			//[RU]: Если, после умножения, значение result больше значения maxNeedExp - уменьшаем значение result до maxNeedExp
			//[RU]: Если, после умножения, значение result больше значения maxNeedExp - выходим из цикла
			//------------------------------------------
			//[EN]: If, after multiplication, the value of 'result' is greater than the value of 'maxNeedExp', decrease the value of 'result' to 'maxNeedExp'.
			//[EN]: If, after multiplication, the value of 'result' is greater than the value of 'maxNeedExp', exit the loop.
			if(result > maxNeedExp)
			{
				result = maxNeedExp;
				break;
			}
		}
		
        return result;
    }
}