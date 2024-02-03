using System.Collections.Generic;
using Terraria.ID;

namespace RazorbladeTyphoonProgress.CustCl;

//[RU]: Вспомогательный класс "EffectsSystem" для категории "Дополнительные эффекты" основного предмета мода
//------------------------------------------
//[EN]: Auxiliary class "EffectsSystem" for the "Additional Effects" category of the main item in the mod
public class EffectsSystem
{
	//[RU]: Максимальный уровень категории "Дополнительные эффекты"
	//------------------------------------------
	//[EN]: Maximum level of the "Additional Effects" category
    public int MaxLevel = 0;
	
	//[RU]: Список, хранящий в себе список всех усилений категории "Дополнительные эффекты"
	//[RU]: Описание типов кортежа смотрите ниже, в описании метода AddEffects()
	//------------------------------------------
	//[EN]: A list containing all enhancements of the "Additional Effects" category
	//[EN]: Refer to the description of tuple types below in the AddEffects() method description
    public List<(int buffID, bool isUsePerKey, bool isAddNPCTarget, bool isNoCollide)> effectsInfo = new();
	
	//[RU]: Запрещаем вызов конструктора (т.е. создание экземпляра данного класса) вне данного класса
	//------------------------------------------
	//[EN]: Prohibiting the invocation of the constructor (i.e., creating an instance of this class) outside of this class
    private EffectsSystem(){}

	//[RU]: Метод, для контролируемого создания экземпляра класса EffectsSystem
	//------------------------------------------
	//[EN]: Method for controlled creation of an instance of the EffectsSystem class
    public static EffectsSystem CreateEffectSystem()
    {
        EffectsSystem es = new();

		//[RU]: Добавляем усиления для категории "Дополнительные эффекты"
		//[RU]: См. описание метода AddEffects ниже
		//------------------------------------------
		//[EN]: Adding enhancements for the "Additional Effects" category
		//[EN]: See the description of the AddEffects method below
		es.AddEffects(BuffID.NightOwl);
		es.AddEffects(BuffID.Shine);
		es.AddEffects(BuffID.Spelunker);
		es.AddEffects(BuffID.Swiftness);
		es.AddEffects(BuffID.Regeneration);
		es.AddEffects(BuffID.Ironskin);
		es.AddEffects(BuffID.Ichor, false, true);
		es.AddEffects(BuffID.ObsidianSkin);
		es.AddEffects(BuffID.Gills);
		es.AddEffects(BuffID.MagicPower);
		es.AddEffects(BuffID.ManaRegeneration);
		es.AddEffects(-1, false, false, true);
		es.AddEffects(BuffID.Rage);

		//[RU]: Присваиваем значение переменной MaxLevel значением кол-во усилением в листе effectsInfo
		//------------------------------------------
		//[EN]: Assigns the value of the variable MaxLevel to the number of enhancements in the effectsInfo list.
        es.MaxLevel = es.effectsInfo.Count - 1;
        return es;
    }

	//[RU]: Возвращает на каком уровне категори "Дополнительные эффекты" открывается доступ к усиление "Снаряды проходят сквозь блоки"
	//------------------------------------------
	//[EN]: Returns at which level the "Additional Effects" category unlocks access to the "Projectiles pass through blocks" enhancement.
    public int GetNoCollideLevel()
    {
        for(int i = 0; i < effectsInfo.Count; i++)
            if(effectsInfo[i].isNoCollide)
                return i + 1;
        return -1;
    }

	//[RU]: Метод, регистрирующий усиления для категории "Дополнительные эффекты"
	//[RU]: buffID - тип баффа, который будет наложен на игрока или NPC
	//[RU]: isUsePerKey - указывает, что данный бафф должен активироваться только по нажатию клавииш (не актуально)
	//[RU]: isAddNPCTarget - указывает, что данный бафф должен применяться к NPC
	//[RU]: isNoCollide - указывает, что это усиление заставляющее снаряды проходить сквозь блоки
	//------------------------------------------
	//[EN]: Method registering enhancements for the "Additional Effects" category
	//[EN]: buffID - the type of buff that will be applied to the player or NPC
	//[EN]: isUsePerKey - indicates that this buff should only be activated by pressing keys (not applicable)
	//[EN]: isAddNPCTarget - indicates that this buff should be applied to NPC
	//[EN]: isNoCollide - indicates that this enhancement makes projectiles pass through blocks
    private void AddEffects(int buffID, bool isUsePerKey = false, bool isAddNPCTarget = false, bool isNoCollide = false)
        => effectsInfo.Add((buffID, isUsePerKey, isAddNPCTarget, isNoCollide));
}