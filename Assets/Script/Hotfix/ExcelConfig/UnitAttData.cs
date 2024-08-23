
using System.Collections.Generic;
using UnityEngine;

namespace Game.Config
{
    public class UnitAttData
    {

        static UnitAttData()
        {
            entityDic = new Dictionary<int, UnitAttEntity>(3);
             UnitAttEntity e0 = new UnitAttEntity(1,10000f,3000f,100f,20f,100f,30f,1.5f,0.8f);
            entityDic.Add(e0.id, e0);
             UnitAttEntity e1 = new UnitAttEntity(2,15000f,100f,100f,20f,100f,50f,1.5f,0.8f);
            entityDic.Add(e1.id, e1);
             UnitAttEntity e2 = new UnitAttEntity(3,23000f,100f,100f,20f,100f,30f,1.5f,0.8f);
            entityDic.Add(e2.id, e2);

        }

       
        
        public static Dictionary<int, UnitAttEntity> all {
            get {
                return entityDic;
            }
        }
		static Dictionary<int, UnitAttEntity> entityDic;
		public static UnitAttEntity Get(int id)
		{
            if (entityDic!=null&&entityDic.TryGetValue(id,out var entity))
			{
				return entity;
			}
            return null;
		}
    }

    
    public class UnitAttEntity
    {
        //TemplateMember
		public int id;//ID
		public float hp;//生命值
		public float phy_atk;//物理攻击
		public float magic_atk;//魔法攻击
		public float phy_def;//物理抗性
		public float magic_def;//魔法抗性
		public float critical_hit_rate;//暴击率
		public float critical_hit_multiple;//暴击伤害加成
		public float skill_speed;//技能急速

        public UnitAttEntity(){}
        public UnitAttEntity(int id,float hp,float phy_atk,float magic_atk,float phy_def,float magic_def,float critical_hit_rate,float critical_hit_multiple,float skill_speed){
           
           this.id = id;
           this.hp = hp;
           this.phy_atk = phy_atk;
           this.magic_atk = magic_atk;
           this.phy_def = phy_def;
           this.magic_def = magic_def;
           this.critical_hit_rate = critical_hit_rate;
           this.critical_hit_multiple = critical_hit_multiple;
           this.skill_speed = skill_speed;

        }
    }
}
