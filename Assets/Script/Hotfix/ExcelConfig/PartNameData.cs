
using System.Collections.Generic;
using UnityEngine;

namespace Game.Config
{
    public class PartNameData
    {

        static PartNameData()
        {
            entityDic = new Dictionary<int, PartNameEntity>(10);
             PartNameEntity e0 = new PartNameEntity(1,@"头盔");
            entityDic.Add(e0.id, e0);
             PartNameEntity e1 = new PartNameEntity(2,@"衣服");
            entityDic.Add(e1.id, e1);
             PartNameEntity e2 = new PartNameEntity(3,@"披风");
            entityDic.Add(e2.id, e2);
             PartNameEntity e3 = new PartNameEntity(4,@"裤子");
            entityDic.Add(e3.id, e3);
             PartNameEntity e4 = new PartNameEntity(5,@"靴子");
            entityDic.Add(e4.id, e4);
             PartNameEntity e5 = new PartNameEntity(6,@"武器");
            entityDic.Add(e5.id, e5);
             PartNameEntity e6 = new PartNameEntity(7,@"耳环");
            entityDic.Add(e6.id, e6);
             PartNameEntity e7 = new PartNameEntity(8,@"项链");
            entityDic.Add(e7.id, e7);
             PartNameEntity e8 = new PartNameEntity(9,@"戒指");
            entityDic.Add(e8.id, e8);
             PartNameEntity e9 = new PartNameEntity(10,@"腰带");
            entityDic.Add(e9.id, e9);

        }

       
        
        public static Dictionary<int, PartNameEntity> all {
            get {
                return entityDic;
            }
        }
		static Dictionary<int, PartNameEntity> entityDic;
		public static PartNameEntity Get(int id)
		{
            if (entityDic!=null&&entityDic.TryGetValue(id,out var entity))
			{
				return entity;
			}
            return null;
		}
    }

    
    public class PartNameEntity
    {
        //TemplateMember
		public int id;//ID
		public string name;//名称

        public PartNameEntity(){}
        public PartNameEntity(int id,string name){
           
           this.id = id;
           this.name = name;

        }
    }
}
