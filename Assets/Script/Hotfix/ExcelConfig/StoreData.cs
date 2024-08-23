
using System.Collections.Generic;
using UnityEngine;

namespace Game.Config
{
    public class StoreData
    {

        static StoreData()
        {
            entityDic = new Dictionary<int, StoreEntity>(24);
             StoreEntity e0 = new StoreEntity(1001,1);
            entityDic.Add(e0.id, e0);
             StoreEntity e1 = new StoreEntity(1002,1);
            entityDic.Add(e1.id, e1);
             StoreEntity e2 = new StoreEntity(1003,1);
            entityDic.Add(e2.id, e2);
             StoreEntity e3 = new StoreEntity(1004,1);
            entityDic.Add(e3.id, e3);
             StoreEntity e4 = new StoreEntity(1005,1);
            entityDic.Add(e4.id, e4);
             StoreEntity e5 = new StoreEntity(2002,1);
            entityDic.Add(e5.id, e5);
             StoreEntity e6 = new StoreEntity(2003,1);
            entityDic.Add(e6.id, e6);
             StoreEntity e7 = new StoreEntity(2004,1);
            entityDic.Add(e7.id, e7);
             StoreEntity e8 = new StoreEntity(2005,1);
            entityDic.Add(e8.id, e8);
             StoreEntity e9 = new StoreEntity(2012,1);
            entityDic.Add(e9.id, e9);
             StoreEntity e10 = new StoreEntity(2013,1);
            entityDic.Add(e10.id, e10);
             StoreEntity e11 = new StoreEntity(2014,1);
            entityDic.Add(e11.id, e11);
             StoreEntity e12 = new StoreEntity(2015,1);
            entityDic.Add(e12.id, e12);
             StoreEntity e13 = new StoreEntity(2016,1);
            entityDic.Add(e13.id, e13);
             StoreEntity e14 = new StoreEntity(2017,1);
            entityDic.Add(e14.id, e14);
             StoreEntity e15 = new StoreEntity(2018,1);
            entityDic.Add(e15.id, e15);
             StoreEntity e16 = new StoreEntity(2019,1);
            entityDic.Add(e16.id, e16);
             StoreEntity e17 = new StoreEntity(2020,1);
            entityDic.Add(e17.id, e17);
             StoreEntity e18 = new StoreEntity(2021,1);
            entityDic.Add(e18.id, e18);
             StoreEntity e19 = new StoreEntity(2022,1);
            entityDic.Add(e19.id, e19);
             StoreEntity e20 = new StoreEntity(2023,1);
            entityDic.Add(e20.id, e20);
             StoreEntity e21 = new StoreEntity(2024,1);
            entityDic.Add(e21.id, e21);
             StoreEntity e22 = new StoreEntity(2025,1);
            entityDic.Add(e22.id, e22);
             StoreEntity e23 = new StoreEntity(2026,1);
            entityDic.Add(e23.id, e23);

        }

       
        
        public static Dictionary<int, StoreEntity> all {
            get {
                return entityDic;
            }
        }
		static Dictionary<int, StoreEntity> entityDic;
		public static StoreEntity Get(int id)
		{
            if (entityDic!=null&&entityDic.TryGetValue(id,out var entity))
			{
				return entity;
			}
            return null;
		}
    }

    
    public class StoreEntity
    {
        //TemplateMember
		public int id;//物品ID
		public int price;//售价

        public StoreEntity(){}
        public StoreEntity(int id,int price){
           
           this.id = id;
           this.price = price;

        }
    }
}
