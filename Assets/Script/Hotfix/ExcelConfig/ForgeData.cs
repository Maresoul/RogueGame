
using System.Collections.Generic;
using UnityEngine;

namespace Game.Config
{
    public class ForgeData
    {

        static ForgeData()
        {
            entityDic = new Dictionary<int, ForgeEntity>(11);
             ForgeEntity e0 = new ForgeEntity(1,2003,new int[]{2001,1},new int[]{3001,1},new int[]{3002,1});
            entityDic.Add(e0.id, e0);
             ForgeEntity e1 = new ForgeEntity(2,2006,new int[]{2004,1},new int[]{3003,1},new int[]{3004,1});
            entityDic.Add(e1.id, e1);
             ForgeEntity e2 = new ForgeEntity(3,2009,new int[]{2007,1},new int[]{3006,1},new int[]{3012,1});
            entityDic.Add(e2.id, e2);
             ForgeEntity e3 = new ForgeEntity(4,2012,new int[]{2010,1},new int[]{3011,1},new int[]{3005,1});
            entityDic.Add(e3.id, e3);
             ForgeEntity e4 = new ForgeEntity(5,2015,new int[]{2013,1},new int[]{3003,2},new int[]{3004,1});
            entityDic.Add(e4.id, e4);
             ForgeEntity e5 = new ForgeEntity(6,2018,new int[]{2016,1},new int[]{3006,2},new int[]{3012,1});
            entityDic.Add(e5.id, e5);
             ForgeEntity e6 = new ForgeEntity(7,2021,new int[]{2019,1},new int[]{3011,2},new int[]{3013,1});
            entityDic.Add(e6.id, e6);
             ForgeEntity e7 = new ForgeEntity(8,2024,new int[]{2022,1},new int[]{3003,3},new int[]{3013,1});
            entityDic.Add(e7.id, e7);
             ForgeEntity e8 = new ForgeEntity(9,2027,new int[]{2025,1},new int[]{3006,3},new int[]{3013,1});
            entityDic.Add(e8.id, e8);
             ForgeEntity e9 = new ForgeEntity(10,2030,new int[]{2028,1},new int[]{3008,1},new int[]{3013,1});
            entityDic.Add(e9.id, e9);
             ForgeEntity e10 = new ForgeEntity(11,2057,new int[]{3013,3},new int[]{3012,5},new int[]{3011,10});
            entityDic.Add(e10.id, e10);

        }

       
        
        public static Dictionary<int, ForgeEntity> all {
            get {
                return entityDic;
            }
        }
		static Dictionary<int, ForgeEntity> entityDic;
		public static ForgeEntity Get(int id)
		{
            if (entityDic!=null&&entityDic.TryGetValue(id,out var entity))
			{
				return entity;
			}
            return null;
		}
    }

    
    public class ForgeEntity
    {
        //TemplateMember
		public int id;//图纸ID
		public int prop_id;//合成物品ID
		public int[] mat1;//合成材料1
		public int[] mat2;//合成材料2
		public int[] mat3;//合成材料3

        public ForgeEntity(){}
        public ForgeEntity(int id,int prop_id,int[] mat1,int[] mat2,int[] mat3){
           
           this.id = id;
           this.prop_id = prop_id;
           this.mat1 = mat1;
           this.mat2 = mat2;
           this.mat3 = mat3;

        }
    }
}
