# 蜂萌Xbox手柄调度模块
## 预览
![image](ReadMeImage\preview.png)
## 开始使用
### 1）Editor > ProjectSettings > Player > OtherSettings > ActiveInputHanding 选项设置为【InputSystemPackage】或者【Both】。
### 2) Window > PackageManager 中确定已安装InputSystem
### 3）在Project面板中的Packages列表里找到【蜂萌Xbox手柄调度模块】> Runtime 中的预制体，拖入游戏的第0个场景中。
### 4）在需要使用手柄排序界面的时候调用以下代码
```csharp
Carinnor.XboxController.ControllerManager(int PlayerNum);
```
### 5）在需要监听手柄按键时调用以下相关函数
```csharp
//按键按下时
GetKeyDown(XboxControllerKey key, int playerIndex);

//按键抬起
GetKeyUp(XboxControllerKey key, int playerIndex);

//按键按住
GetKey(XboxControllerKey key, int playerIndex);

//轴的数值
GetAxisValue(XboxControllerKey key, int playerIndex);

//水平轴数值
GetHorizontalValue(int playerIndex, bool isLeftStick);

//垂直轴数值
GetVerticalValue(int playerIndex, bool isLeftStick);

//设置马达震度速度，仅pc生效
SetMotorSpeeds(float left, float right, int playerIndex);

//清除所有马达震动
ClearAllMotorSpeed();


//额外功能
RealIndex //存储了真实手柄顺序，比如第0个玩家目前使用的手柄索引即 RealIndex[0] 可以由此推测玩家使用的屏幕
```