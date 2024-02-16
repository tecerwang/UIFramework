# UIFramewrok.master

#### 介绍
UI Screen&Popup 显示框架

#### 软件架构
UnityEngine

#### UnityPackage Dependencies
UniTask  
Addressables  

#### 使用说明

1.UIScreenManager：管理 scene 中所有 UIScreenBase 实例;  
2.UIScreenBase：基于 UnityEngine.UI.Canvas 的 Overlay 模式创建的 gameObject，管理 UIPopupBase;  
3.UIPopupBase：显示在 canvas 上的 Popup;  
4.所有资源加载基于 Unity Addressable 资源加载框架;  
5.基于 UniTask 异步功能的代码风格； 

#### Unity package import
https://github.com/tecerwang/UIFramework?path=Assets/UIFramework