# RogueGame

## 简介

基于Unity6的DOTS框架开发的一款肉鸽弹幕游戏,支持海量怪物同屏，实现Rogue游戏中的移动射击，敌人随机生成，弹幕躲避，多样化的武器和能力，道具升级等常见要素

[**完整项目演示**](https://www.bilibili.com/video/BV1gUWjeUETc/?spm_id_from=333.999.0.0&vd_source=15ce64d8f8fad36086523ce711dec730)

[**游戏下载地址**](https://pan.baidu.com/s/1Q-cIa8th9wWRaVGtJA1YDg?pwd=67qf)

## 技术点

1. 混合架构开发，Unity6 Dots框架下，ECS架构和面向对象GameObject共同开发游戏逻辑；
2. GPU ECS AnimationBaker插件实现GPU动画的解决方案,降低CPU骨骼计算压力,实现上百人同屏动画播放；
3. ORCA即(RVO2)算法实现动态避障,避免敌人寻路出现重叠的情况；
4. JobSystem高效并行处理怪物生成，子弹创建、飞行、命中检测，特效生成等逻辑；
5. Excel数据配置 +导表工具 + Scriptable Objects 实现高效的数据管理（主角状态机+子弹、特效、动画管理）
6. 多层Animator+IK Anim插件实现角色的跑动射击

## 压力测试

在上百怪物同屏场景下，帧率能稳定在80~90帧（开启录屏软件），释放炸弹等特效时帧率会降至70帧左右。
测试环境：i5-12600kF + RTX 4060ti，如下图所示：

![压力测试](https://github.com/Maresoul/RogueGame/blob/main/%E5%8E%8B%E5%8A%9B%E6%B5%8B%E8%AF%95-1.gif)
