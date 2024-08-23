
using System.Collections.Generic;
using UnityEngine;

namespace Game.Config
{
    public class UnitData
    {

        static UnitData()
        {
            entityDic = new Dictionary<int, UnitEntity>(4);
             UnitEntity e0 = new UnitEntity(1001,@"玄影剑姬",0,0,0,1,10011,10012,10013,10014,10015,10016,10017,10018,10019,10020,10021,10022,80,60,30,50,30);
            entityDic.Add(e0.id, e0);
             UnitEntity e1 = new UnitEntity(1002,@"红焰邪姬",1,1,0,2,20011,20012,20013,20014,20015,20016,20017,20018,20011,20011,20011,20011,80,60,30,50,20);
            entityDic.Add(e1.id, e1);
             UnitEntity e2 = new UnitEntity(1003,@"独目锤影",3,1,1,2,30011,30012,30013,30014,30015,30016,30017,30018,30011,30011,30011,30011,80,60,30,50,20);
            entityDic.Add(e2.id, e2);
             UnitEntity e3 = new UnitEntity(1004,@"小兵C",3,1,0,2,20011,20012,20013,20014,20015,20016,20017,20018,20011,20011,20011,20011,80,60,30,50,20);
            entityDic.Add(e3.id, e3);

        }

       
        
        public static Dictionary<int, UnitEntity> all {
            get {
                return entityDic;
            }
        }
		static Dictionary<int, UnitEntity> entityDic;
		public static UnitEntity Get(int id)
		{
            if (entityDic!=null&&entityDic.TryGetValue(id,out var entity))
			{
				return entity;
			}
            return null;
		}
    }

    
    public class UnitEntity
    {
        //TemplateMember
		public int id;//单位ID
		public string info;//说明
		public int type;//类型
		public int camp;//阵营
		public int atk_type;//攻击定位
		public int att_id;//属性表ID
		public int ntk1;//技能ID_普攻1
		public int ntk2;//技能ID_普攻2
		public int ntk3;//技能ID_普攻3
		public int ntk4;//技能ID_普攻4
		public int skill1;//技能ID_技能1
		public int skill2;//技能ID_技能2
		public int skill3;//技能ID_技能3
		public int skill4;//技能ID_技能4
		public int use_prop_1;//使用暗器_炸弹
		public int use_prop_2;//使用暗器_飞镖
		public int use_prop_3;//使用暗器_飞刀
		public int use_prop_4;//使用暗器_圆轮
		public int block_probability;//格挡概率
		public int dodge_probability;//躲闪概率
		public int atk_probability;//对拼概率
		public int active_attack_probability;//主动发起攻击概率
		public int pacing_probability;//踱步概率

        public UnitEntity(){}
        public UnitEntity(int id,string info,int type,int camp,int atk_type,int att_id,int ntk1,int ntk2,int ntk3,int ntk4,int skill1,int skill2,int skill3,int skill4,int use_prop_1,int use_prop_2,int use_prop_3,int use_prop_4,int block_probability,int dodge_probability,int atk_probability,int active_attack_probability,int pacing_probability){
           
           this.id = id;
           this.info = info;
           this.type = type;
           this.camp = camp;
           this.atk_type = atk_type;
           this.att_id = att_id;
           this.ntk1 = ntk1;
           this.ntk2 = ntk2;
           this.ntk3 = ntk3;
           this.ntk4 = ntk4;
           this.skill1 = skill1;
           this.skill2 = skill2;
           this.skill3 = skill3;
           this.skill4 = skill4;
           this.use_prop_1 = use_prop_1;
           this.use_prop_2 = use_prop_2;
           this.use_prop_3 = use_prop_3;
           this.use_prop_4 = use_prop_4;
           this.block_probability = block_probability;
           this.dodge_probability = dodge_probability;
           this.atk_probability = atk_probability;
           this.active_attack_probability = active_attack_probability;
           this.pacing_probability = pacing_probability;

        }
    }
}
