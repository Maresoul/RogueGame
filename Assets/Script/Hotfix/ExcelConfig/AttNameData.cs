
using System.Collections.Generic;
using UnityEngine;

namespace Game.Config
{
    public class AttNameData
    {

        static AttNameData()
        {
            entityDic = new Dictionary<int, AttNameEntity>(8);
             AttNameEntity e0 = new AttNameEntity(1,@"生命值");
            entityDic.Add(e0.id, e0);
             AttNameEntity e1 = new AttNameEntity(2,@"物理攻击");
            entityDic.Add(e1.id, e1);
             AttNameEntity e2 = new AttNameEntity(3,@"魔法攻击");
            entityDic.Add(e2.id, e2);
             AttNameEntity e3 = new AttNameEntity(4,@"物理抗性");
            entityDic.Add(e3.id, e3);
             AttNameEntity e4 = new AttNameEntity(5,@"魔法抗性");
            entityDic.Add(e4.id, e4);
             AttNameEntity e5 = new AttNameEntity(6,@"暴击率");
            entityDic.Add(e5.id, e5);
             AttNameEntity e6 = new AttNameEntity(7,@"暴击伤害加成");
            entityDic.Add(e6.id, e6);
             AttNameEntity e7 = new AttNameEntity(8,@"技能急速");
            entityDic.Add(e7.id, e7);

        }

       
        
        public static Dictionary<int, AttNameEntity> all {
            get {
                return entityDic;
            }
        }
		static Dictionary<int, AttNameEntity> entityDic;
		public static AttNameEntity Get(int id)
		{
            if (entityDic!=null&&entityDic.TryGetValue(id,out var entity))
			{
				return entity;
			}
            return null;
		}
    }

    
    public class AttNameEntity
    {
        //TemplateMember
		public int id;//ID
		public string name;//名称

        public AttNameEntity(){}
        public AttNameEntity(int id,string name){
           
           this.id = id;
           this.name = name;

        }
    }
}
