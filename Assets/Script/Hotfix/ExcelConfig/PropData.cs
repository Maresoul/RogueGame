
using System.Collections.Generic;
using UnityEngine;

namespace Game.Config
{
    public class PropData
    {

        static PropData()
        {
            entityDic = new Dictionary<int, PropEntity>(84);
             PropEntity e0 = new PropEntity(1001,0,@"啤酒",@"清爽饮品，麦芽香气四溢。",1,@"Beer1",0,new int[]{1,50,0,0,0},new int[]{2,30,1,50,30},new int[]{4,30,1,50,30},0,null,null,null,null,null,0);
            entityDic.Add(e0.id, e0);
             PropEntity e1 = new PropEntity(1002,0,@"高级鱼肉",@"精选鱼肉，鲜嫩且口感极佳。",1,@"Fish2",0,new int[]{1,53,0,0,0},new int[]{2,10,1,42,30},new int[]{4,30,1,50,30},0,null,null,null,null,null,0);
            entityDic.Add(e1.id, e1);
             PropEntity e2 = new PropEntity(1003,0,@"精良肉片",@"刀工精湛，肉质鲜美可口。",1,@"Meat5",0,new int[]{1,50,0,0,0},new int[]{2,30,1,50,30},new int[]{4,30,1,50,30},0,null,null,null,null,null,0);
            entityDic.Add(e2.id, e2);
             PropEntity e3 = new PropEntity(1004,0,@"美味鸡腿",@"鸡腿肉质鲜嫩，口感香滑。",1,@"Meat7",0,new int[]{1,50,0,0,0},null,new int[]{4,30,1,50,30},0,null,null,null,null,null,0);
            entityDic.Add(e3.id, e3);
             PropEntity e4 = new PropEntity(1005,0,@"高级兽肉",@"珍稀兽肉，肉质细腻多汁。",1,@"Meat12",0,new int[]{1,50,0,0,0},new int[]{2,30,1,50,30},new int[]{4,30,1,50,30},0,null,null,null,null,null,0);
            entityDic.Add(e4.id, e4);
             PropEntity e5 = new PropEntity(1006,0,@"普通烤排",@"经典烤肉，外焦里嫩香气足。",1,@"Meat8",0,null,new int[]{2,30,1,50,30},new int[]{4,30,1,50,30},0,null,null,null,null,null,0);
            entityDic.Add(e5.id, e5);
             PropEntity e6 = new PropEntity(1007,0,@"高级烤排",@"优质烤肉，口感与味道上乘。",1,@"Meat4",0,new int[]{1,50,0,0,0},new int[]{2,30,1,50,30},new int[]{4,30,1,50,30},0,null,null,null,null,null,0);
            entityDic.Add(e6.id, e6);
             PropEntity e7 = new PropEntity(1008,0,@"普通鸭腿",@"家常鸭腿，肉香味美。",1,@"Meat13",0,new int[]{1,50,0,0,0},new int[]{2,30,1,50,30},null,0,null,null,null,null,null,0);
            entityDic.Add(e7.id, e7);
             PropEntity e8 = new PropEntity(1009,0,@"华夫饼",@"香甜可口，松软细腻的点心。",1,@"Waffle1",0,null,new int[]{2,30,1,50,30},new int[]{4,30,1,50,30},0,null,null,null,null,null,0);
            entityDic.Add(e8.id, e8);
             PropEntity e9 = new PropEntity(1010,0,@"辣椒",@"香辣刺激，提升菜肴风味的调料。",1,@"Vegetable3",0,new int[]{1,50,0,0,0},new int[]{2,30,1,50,30},new int[]{4,30,1,50,30},0,null,null,null,null,null,0);
            entityDic.Add(e9.id, e9);
             PropEntity e10 = new PropEntity(1011,0,@"飞镖",@"命中单位时对单位造成一定的伤害。",1,@"Dart01",0,null,null,null,0,null,null,null,null,null,1046);
            entityDic.Add(e10.id, e10);
             PropEntity e11 = new PropEntity(1012,0,@"炸弹",@"可以对一定范围内的单位造成伤害。",1,@"Bomb01",0,null,null,null,0,null,null,null,null,null,1047);
            entityDic.Add(e11.id, e11);
             PropEntity e12 = new PropEntity(1013,0,@"飞刀",@"可以穿透单位,对单位造成一定的伤害。",1,@"Knife01",0,null,null,null,0,null,null,null,null,null,1048);
            entityDic.Add(e12.id, e12);
             PropEntity e13 = new PropEntity(1014,0,@"圆轮",@"高速旋转,对单位造成大量的伤害。",1,@"RoundWheel01",0,null,null,null,0,null,null,null,null,null,1049);
            entityDic.Add(e13.id, e13);
             PropEntity e14 = new PropEntity(2001,1,@"野兽披风-普通",@"野性霸气，彰显力量与威严。",0,@"Cloak1",0,null,null,null,3,new int[]{1,50,0},new int[]{6,20,1},null,new int[]{1,20,1},new int[]{1,80,0},0);
            entityDic.Add(e14.id, e14);
             PropEntity e15 = new PropEntity(2002,1,@"野兽披风-精良",@"野性霸气，彰显力量与威严。",0,@"Cloak2",0,null,null,null,3,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e15.id, e15);
             PropEntity e16 = new PropEntity(2003,1,@"野兽披风-稀有",@"野性霸气，彰显力量与威严。",0,@"Cloak3",0,null,null,null,3,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e16.id, e16);
             PropEntity e17 = new PropEntity(2004,1,@"无尽披风-普通",@"深邃神秘，流动着无尽之力。",0,@"Cloak7",0,null,null,null,3,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e17.id, e17);
             PropEntity e18 = new PropEntity(2005,1,@"无尽披风-精良",@"深邃神秘，流动着无尽之力。",0,@"Cloak8",0,null,null,null,3,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e18.id, e18);
             PropEntity e19 = new PropEntity(2006,1,@"无尽披风-稀有",@"深邃神秘，流动着无尽之力。",0,@"Cloak9",0,null,null,null,3,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e19.id, e19);
             PropEntity e20 = new PropEntity(2007,1,@"野兽腰带-普通",@"环绕腰间，野兽之魂相伴左右。",0,@"BeastHunter_Belt_1",0,null,null,null,10,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e20.id, e20);
             PropEntity e21 = new PropEntity(2008,1,@"野兽腰带-精良",@"环绕腰间，野兽之魂相伴左右。",0,@"BeastHunter_Belt_2",0,null,null,null,10,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e21.id, e21);
             PropEntity e22 = new PropEntity(2009,1,@"野兽腰带-稀有",@"环绕腰间，野兽之魂相伴左右。",0,@"BeastHunter_Belt_3",0,null,null,null,10,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e22.id, e22);
             PropEntity e23 = new PropEntity(2010,1,@"恶魔腰带-普通",@"恶魔之力，瞬间提升战斗气息。",0,@"DemonHunter_Belt_1",0,null,null,null,10,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e23.id, e23);
             PropEntity e24 = new PropEntity(2011,1,@"恶魔腰带-精良",@"恶魔之力，瞬间提升战斗气息。",0,@"DemonHunter_Belt_2",0,null,null,null,10,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e24.id, e24);
             PropEntity e25 = new PropEntity(2012,1,@"恶魔腰带-稀有",@"恶魔之力，瞬间提升战斗气息。",0,@"DemonHunter_Belt_3",0,null,null,null,10,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e25.id, e25);
             PropEntity e26 = new PropEntity(2013,1,@"野兽猎人靴-普通",@"稳固防滑，助力猎人追踪野兽。",0,@"BeastHunter_Boots_1",0,null,null,null,5,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e26.id, e26);
             PropEntity e27 = new PropEntity(2014,1,@"野兽猎人靴-精良",@"稳固防滑，助力猎人追踪野兽。",0,@"BeastHunter_Boots_2",0,null,null,null,5,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e27.id, e27);
             PropEntity e28 = new PropEntity(2015,1,@"野兽猎人靴-稀有",@"稳固防滑，助力猎人追踪野兽。",0,@"BeastHunter_Boots_3",0,null,null,null,5,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e28.id, e28);
             PropEntity e29 = new PropEntity(2016,1,@"野兽猎人衣-普通",@"隐秘伪装，与野兽斗智斗勇。",0,@"BeastHunter_Chest_1",0,null,null,null,2,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e29.id, e29);
             PropEntity e30 = new PropEntity(2017,1,@"野兽猎人衣-精良",@"隐秘伪装，与野兽斗智斗勇。",0,@"BeastHunter_Chest_2",0,null,null,null,2,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e30.id, e30);
             PropEntity e31 = new PropEntity(2018,1,@"野兽猎人衣-稀有",@"隐秘伪装，与野兽斗智斗勇。",0,@"BeastHunter_Chest_3",0,null,null,null,2,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e31.id, e31);
             PropEntity e32 = new PropEntity(2019,1,@"野猪胸甲-普通",@"厚实的皮甲，提供极高的抗性。",0,@"Boar_Chest_1",0,null,null,null,2,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e32.id, e32);
             PropEntity e33 = new PropEntity(2020,1,@"野猪胸甲-精良",@"厚实的皮甲，提供极高的抗性。",0,@"Boar_Chest_2",0,null,null,null,2,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e33.id, e33);
             PropEntity e34 = new PropEntity(2021,1,@"野猪胸甲-稀有",@"厚实的皮甲，提供极高的抗性。",0,@"Boar_Chest_3",0,null,null,null,2,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e34.id, e34);
             PropEntity e35 = new PropEntity(2022,1,@"清风道服-普通",@"轻盈飘逸，如清风般舒适自在。",0,@"Cloth2_Chest_1",0,null,null,null,2,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e35.id, e35);
             PropEntity e36 = new PropEntity(2023,1,@"清风道服-精良",@"轻盈飘逸，如清风般舒适自在。",0,@"Cloth2_Chest_2",0,null,null,null,2,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e36.id, e36);
             PropEntity e37 = new PropEntity(2024,1,@"清风道服-稀有",@"轻盈飘逸，如清风般舒适自在。",0,@"Cloth2_Chest_3",0,null,null,null,2,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e37.id, e37);
             PropEntity e38 = new PropEntity(2025,1,@"轻者耳环-普通",@"轻盈灵动，耳环闪耀添魅力。",0,@"Earring1",0,null,null,null,7,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e38.id, e38);
             PropEntity e39 = new PropEntity(2026,1,@"轻者耳环-精良",@"轻盈灵动，耳环闪耀添魅力。",0,@"Earring2",0,null,null,null,7,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e39.id, e39);
             PropEntity e40 = new PropEntity(2027,1,@"轻者耳环-稀有",@"轻盈灵动，耳环闪耀添魅力。",0,@"Earring3",0,null,null,null,7,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e40.id, e40);
             PropEntity e41 = new PropEntity(2028,1,@"猎人头盔-普通",@"狩猎专属，头盔坚固护头脑。",0,@"Cloth_Helmet1",0,null,null,null,1,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e41.id, e41);
             PropEntity e42 = new PropEntity(2029,1,@"猎人头盔-精良",@"狩猎专属，头盔坚固护头脑。",0,@"Cloth_Helmet2",0,null,null,null,1,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e42.id, e42);
             PropEntity e43 = new PropEntity(2030,1,@"猎人头盔-稀有",@"狩猎专属，头盔坚固护头脑。",0,@"Cloth_Helmet3",0,null,null,null,1,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e43.id, e43);
             PropEntity e44 = new PropEntity(2031,1,@"神者项链-普通",@"神光熠熠，项链护身赐神力。",0,@"Necklace1",0,null,null,null,8,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e44.id, e44);
             PropEntity e45 = new PropEntity(2032,1,@"神者项链-精良",@"神光熠熠，项链护身赐神力。",0,@"Necklace2",0,null,null,null,8,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e45.id, e45);
             PropEntity e46 = new PropEntity(2033,1,@"神者项链-稀有",@"神光熠熠，项链护身赐神力。",0,@"Necklace3",0,null,null,null,8,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e46.id, e46);
             PropEntity e47 = new PropEntity(2034,1,@"野猪长裤-普通",@"野猪皮制，长裤坚韧护双腿。",0,@"Boar_Pants_1",0,null,null,null,4,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e47.id, e47);
             PropEntity e48 = new PropEntity(2035,1,@"野猪长裤-精良",@"野猪皮制，长裤坚韧护双腿。",0,@"Boar_Pants_2",0,null,null,null,4,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e48.id, e48);
             PropEntity e49 = new PropEntity(2036,1,@"野猪长裤-稀有",@"野猪皮制，长裤坚韧护双腿。",0,@"Boar_Pants_3",0,null,null,null,4,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e49.id, e49);
             PropEntity e50 = new PropEntity(2037,1,@"神者长裤-普通",@"神灵加护，长裤飘逸显英姿。",0,@"Corrupted_Pants_1",0,null,null,null,4,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e50.id, e50);
             PropEntity e51 = new PropEntity(2038,1,@"神者长裤-精良",@"神灵加护，长裤飘逸显英姿。",0,@"Corrupted_Pants_2",0,null,null,null,4,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e51.id, e51);
             PropEntity e52 = new PropEntity(2039,1,@"神者长裤-稀有",@"神灵加护，长裤飘逸显英姿。",0,@"Corrupted_Pants_3",0,null,null,null,4,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e52.id, e52);
             PropEntity e53 = new PropEntity(2040,1,@"神者戒指-普通",@"神力环绕，戒指闪耀显神威。",0,@"Ring1",0,null,null,null,9,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e53.id, e53);
             PropEntity e54 = new PropEntity(2041,1,@"神者戒指-精良",@"神力环绕，戒指闪耀显神威。",0,@"Ring2",0,null,null,null,9,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e54.id, e54);
             PropEntity e55 = new PropEntity(2042,1,@"神者戒指-稀有",@"神力环绕，戒指闪耀显神威。",0,@"Ring3",0,null,null,null,9,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e55.id, e55);
             PropEntity e56 = new PropEntity(2043,1,@"猎人之刃-普通",@"猎人专属，利刃在手猎四方。",0,@"Sword1_1_1",0,null,null,null,6,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e56.id, e56);
             PropEntity e57 = new PropEntity(2044,1,@"猎人之刃-精良",@"猎人专属，利刃在手猎四方。",0,@"Sword1_1_2",0,null,null,null,6,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e57.id, e57);
             PropEntity e58 = new PropEntity(2045,1,@"猎人之刃-稀有",@"猎人专属，利刃在手猎四方。",0,@"Sword1_1_3",0,null,null,null,6,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e58.id, e58);
             PropEntity e59 = new PropEntity(2046,1,@"黑石神刃-普通",@"黑石铸就，神刃无匹震四方。",0,@"Sword1_2_1",0,null,null,null,6,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e59.id, e59);
             PropEntity e60 = new PropEntity(2047,1,@"黑石神刃-精良",@"黑石铸就，神刃无匹震四方。",0,@"Sword1_2_2",0,null,null,null,6,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e60.id, e60);
             PropEntity e61 = new PropEntity(2048,1,@"黑石神刃-稀有",@"黑石铸就，神刃无匹震四方。",0,@"Sword1_2_3",0,null,null,null,6,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e61.id, e61);
             PropEntity e62 = new PropEntity(2049,1,@"黎明之刃-普通",@"破晓时分，利刃闪耀迎曙光。",0,@"Sword1_3_1",0,null,null,null,6,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e62.id, e62);
             PropEntity e63 = new PropEntity(2050,1,@"黎明之刃-精良",@"破晓时分，利刃闪耀迎曙光。",0,@"Sword1_3_2",0,null,null,null,6,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e63.id, e63);
             PropEntity e64 = new PropEntity(2051,1,@"黎明之刃-稀有",@"破晓时分，利刃闪耀迎曙光。",0,@"Sword1_3_3",0,null,null,null,6,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e64.id, e64);
             PropEntity e65 = new PropEntity(2052,1,@"暗黑之刃-普通",@"黑暗之中，锋芒毕露显杀机。",0,@"Sword1_4_1",0,null,null,null,6,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e65.id, e65);
             PropEntity e66 = new PropEntity(2053,1,@"暗黑之刃-精良",@"黑暗之中，锋芒毕露显杀机。",0,@"Sword1_4_2",0,null,null,null,6,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e66.id, e66);
             PropEntity e67 = new PropEntity(2054,1,@"暗黑之刃-稀有",@"黑暗之中，锋芒毕露显杀机。",0,@"Sword1_4_3",0,null,null,null,6,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e67.id, e67);
             PropEntity e68 = new PropEntity(2055,1,@"樱舞之刃-普通",@"樱花飘落，利刃舞动映春光。",0,@"Sword2_1_1",0,null,null,null,6,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e68.id, e68);
             PropEntity e69 = new PropEntity(2056,1,@"樱舞之刃-精良",@"樱花飘落，利刃舞动映春光。",0,@"Sword2_1_2",0,null,null,null,6,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e69.id, e69);
             PropEntity e70 = new PropEntity(2057,1,@"樱舞之刃-稀有",@"樱花飘落，利刃舞动映春光。",0,@"Sword2_1_3",0,null,null,null,6,new int[]{1,50,0},new int[]{6,20,1},new int[]{8,10,0},new int[]{1,30,0},new int[]{1,30,0},0);
            entityDic.Add(e70.id, e70);
             PropEntity e71 = new PropEntity(3001,2,@"普通布料",@"柔软舒适的常用面料。",1,@"Cloth1",0,null,null,null,0,null,null,null,null,null,0);
            entityDic.Add(e71.id, e71);
             PropEntity e72 = new PropEntity(3002,2,@"高级绸缎",@"光滑细腻，质地光亮的绸缎面料。",1,@"Cloth2",0,null,null,null,0,null,null,null,null,null,0);
            entityDic.Add(e72.id, e72);
             PropEntity e73 = new PropEntity(3003,2,@"低级兽骨",@"常见兽类的骨骼，用途广泛。",1,@"Bone1",0,null,null,null,0,null,null,null,null,null,0);
            entityDic.Add(e73.id, e73);
             PropEntity e74 = new PropEntity(3004,2,@"普通头骨",@"普通动物的头骨，常见于野外或者猎人的背包。",1,@"Bone_Skull2",0,null,null,null,0,null,null,null,null,null,0);
            entityDic.Add(e74.id, e74);
             PropEntity e75 = new PropEntity(3005,2,@"高级兽骨",@"珍稀兽类的骨骼，质地非常坚硬。",1,@"Bone_Skull3",0,null,null,null,0,null,null,null,null,null,0);
            entityDic.Add(e75.id, e75);
             PropEntity e76 = new PropEntity(3006,2,@"兽皮",@"动物皮毛，可用于制作衣物或装饰。",1,@"Fur4",0,null,null,null,0,null,null,null,null,null,0);
            entityDic.Add(e76.id, e76);
             PropEntity e77 = new PropEntity(3007,2,@"蓝色宝石",@"深邃蓝色的珍贵宝石。",1,@"Gem6",0,null,null,null,0,null,null,null,null,null,0);
            entityDic.Add(e77.id, e77);
             PropEntity e78 = new PropEntity(3008,2,@"绿色宝石",@"鲜艳绿色的璀璨宝石。",1,@"Gem7",0,null,null,null,0,null,null,null,null,null,0);
            entityDic.Add(e78.id, e78);
             PropEntity e79 = new PropEntity(3009,2,@"金色宝石",@"闪耀着金色光芒的宝石。",1,@"Gem8",0,null,null,null,0,null,null,null,null,null,0);
            entityDic.Add(e79.id, e79);
             PropEntity e80 = new PropEntity(3010,2,@"紫色宝石",@"神秘而高贵的紫色宝石。",1,@"Gem9",0,null,null,null,0,null,null,null,null,null,0);
            entityDic.Add(e80.id, e80);
             PropEntity e81 = new PropEntity(3011,2,@"红色宝石",@"热情如火的红色珍贵宝石。",1,@"Gem10",0,null,null,null,0,null,null,null,null,null,0);
            entityDic.Add(e81.id, e81);
             PropEntity e82 = new PropEntity(3012,2,@"高级金矿",@"高纯度的金色矿石，价值很高。",1,@"Ore_Gold1",0,null,null,null,0,null,null,null,null,null,0);
            entityDic.Add(e82.id, e82);
             PropEntity e83 = new PropEntity(3013,2,@"高级银矿",@"纯净的银色矿石，稀有且珍贵。",1,@"Ore_Silver1",0,null,null,null,0,null,null,null,null,null,0);
            entityDic.Add(e83.id, e83);

        }

       
        
        public static Dictionary<int, PropEntity> all {
            get {
                return entityDic;
            }
        }
		static Dictionary<int, PropEntity> entityDic;
		public static PropEntity Get(int id)
		{
            if (entityDic!=null&&entityDic.TryGetValue(id,out var entity))
			{
				return entity;
			}
            return null;
		}
    }

    
    public class PropEntity
    {
        //TemplateMember
		public int id;//道具ID
		public int type;//类型
		public string name;//名称
		public string info;//介绍
		public int superposition;//是否允许叠加
		public string icon;//图标
		public int level;//使用等级限制
		public int[] recover1;//使用后恢复1
		public int[] recover2;//使用后恢复2
		public int[] recover3;//使用后恢复3
		public int part;//部位
		public int[] att1;//词条1
		public int[] att2;//词条2
		public int[] att3;//词条3
		public int[] att4;//词条4
		public int[] att5;//词条5
		public int chang_state;//切换角色状态

        public PropEntity(){}
        public PropEntity(int id,int type,string name,string info,int superposition,string icon,int level,int[] recover1,int[] recover2,int[] recover3,int part,int[] att1,int[] att2,int[] att3,int[] att4,int[] att5,int chang_state){
           
           this.id = id;
           this.type = type;
           this.name = name;
           this.info = info;
           this.superposition = superposition;
           this.icon = icon;
           this.level = level;
           this.recover1 = recover1;
           this.recover2 = recover2;
           this.recover3 = recover3;
           this.part = part;
           this.att1 = att1;
           this.att2 = att2;
           this.att3 = att3;
           this.att4 = att4;
           this.att5 = att5;
           this.chang_state = chang_state;

        }
    }
}
