using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.XInput;

namespace Reborn.XboxController
{
    public class ControllerManager : MonoBehaviour
    {

        public static ControllerManager Instance;
        
        #region Private

        int DevicesCount = 0;
        List<XInputController> xinputs = new List<XInputController>();
        List<int> xinputIds = new List<int>();
      
        private bool IsControllerSorting = false;
        private Delayer delayer;
        private int ConfirmCounter = 0;
        
        #endregion
        
        
        
        /// <summary>
        /// 真实的手柄索引
        /// </summary>
        public List<int> RealIndex = new List<int>();

        public ControllerSortCanvas[] Canvas;
        
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            Canvas = GetComponentsInChildren<ControllerSortCanvas>();
            
            delayer = new Delayer(this);;
           

            DontDestroyOnLoad(gameObject);
            RefreshXboxDevice();
            SortAsDefault();

            DevicesCount = InputSystem.devices.Count;
        }
        
        /// <summary>
        /// 打开手柄排序界面
        /// </summary>
        /// <param name="PlayerNum">需要排序的玩家人数</param>
        public void OpenControllerSortPanel(int playerNum)
        {
            if(IsControllerSorting)
                return;
            
            ClearAllMotorSpeed();
            ConfirmCounter = playerNum;
            RealIndex = new List<int>();
            for (int i = 0; i < playerNum; i++)
            {
                RealIndex.Add(-1);
            }
            IsControllerSorting = true;
            for (var i = 0; i < Canvas.Length; i++)
            {
                Canvas[i].ShowPanel(playerNum);
            }
        }
        

        private void SortAsDefault()
        {
            xinputs.Sort((a, b) =>
            {
                if (a.name.Length < b.name.Length)
                    return -1;
                else if (a.name.Length > b.name.Length)
                    return 1;
                else
                {
                    var acharArray = a.name.ToCharArray();
                    var bcharArray = b.name.ToCharArray();
                    return acharArray[^1] < bcharArray[^1] ? -1 : 1;
                }
            });
            for (int i = 0; i < xinputs.Count; i++)
            {
                RealIndex.Add(i);
            }
        }

        private void Update()
        {
            if (InputSystem.devices.Count != DevicesCount)
            {
                RefreshXboxDevice();
            }

            if (IsControllerSorting)
            {
                //检测所有手柄的按钮。
                for (var i = 0; i < xinputs.Count; i++)
                {
                    var controller = xinputs[i];
                    if (controller == null)
                        continue;

                    if (controller.leftTrigger.isPressed && controller.rightTrigger.isPressed && !RealIndex.Contains(i))
                    {
                        for (var j = 0; j < RealIndex.Count; j++)
                        {
                            if (RealIndex[j] == -1)
                            {
                                RealIndex[j] = i;
                                var index = j;
                                
                                //图标显示
                                for (var k = 0; k < Canvas.Length; k++)
                                {
                                    Canvas[k].SwitchPlayerGridState(PlayerGridState.Touched,index);
                                }
                                

                                //震动反馈
                                SetMotorSpeeds(1, 1, index);
                                delayer.DelayExecute(0.2f, (a) =>
                                {
                                    SetMotorSpeeds(0, 0, index);
                                });
                                break;
                            }
                        }
                    }

                    if (controller.aButton.wasPressedThisFrame)
                    {
                        for (var j = 0; j < RealIndex.Count; j++)
                        {
                            if (RealIndex[j] == i)
                            {
                                var index = j;
                                
                                //图标显示
                                for (var k = 0; k < Canvas.Length; k++)
                                {
                                    Canvas[k].SwitchPlayerGridState(PlayerGridState.Confirmed,index);
                                }

                                //震动反馈
                                SetMotorSpeeds(1, 1, index);
                                delayer.DelayExecute(0.2f, (a) =>
                                {
                                    SetMotorSpeeds(0, 0, index);
                                });
                                ConfirmCounter--;
                                break;
                            }
                        }
                    }
                    
                    if (controller.bButton.wasPressedThisFrame)
                    {
                        for (var j = 0; j < RealIndex.Count; j++)
                        {
                            if (RealIndex[j] == i)
                            {
                               
                                
                                RealIndex[j] = -1;
                                //图标显示
                                for (var k = 0; k < Canvas.Length; k++)
                                {
                                    Canvas[k].SwitchPlayerGridState(PlayerGridState.Empty,j);
                                }

                                var index = i;
                                //震动反馈
                                _SetMotorSpeeds(1, 1, index);
                                delayer.DelayExecute(0.2f, (a) =>
                                {
                                    _SetMotorSpeeds(0, 0, index);
                                });
                                ConfirmCounter++;
                                break;
                            }
                        }
                    }

                }

                if (ConfirmCounter == 0)
                {
                    for (var i = 0; i < Canvas.Length; i++)
                    {
                        Canvas[i].ClosePanel();
                    }
                    IsControllerSorting = false;
                }
            }

           
        }

        private List<XInputController> _xinputs = new List<XInputController>();

        private void RefreshXboxDevice()
        {
            DevicesCount = InputSystem.devices.Count;
            _xinputs.Clear();

            foreach (var inputDevice in InputSystem.devices)
            {
                if (inputDevice is XInputController)
                {
                    _xinputs.Add((XInputController)inputDevice);
                    Debug.Log(inputDevice.name);
                }
            }

            //移除控制器，但是不打乱控制器顺序
            for (var i = 0; i < xinputs.Count; i++)
            {
                var deviceId = xinputIds[i];
                var flag = false;
                for (var j = 0; j < _xinputs.Count; j++)
                {
                    if (deviceId == _xinputs[j].deviceId)
                    {
                        flag = true;
                        _xinputs.RemoveAt(j);
                        break;
                    }
                }

                if (!flag)
                {
                    xinputs[i] = null;
                    xinputIds[i] = 0;
                }
            }

            //新增控制器，优先插入到空位
            for (var i = 0; i < _xinputs.Count; i++)
            {
                var flag = false;
                for (var j = 0; j < xinputs.Count; j++)
                {
                    if (xinputs[j] == null)
                    {
                        xinputs[j] = _xinputs[i];
                        xinputIds[j] = _xinputs[i].deviceId;
                        flag = true;
                        break;
                    }
                }

                if (!flag)
                {
                    xinputs.Add(_xinputs[i]);
                    xinputIds.Add(_xinputs[i].deviceId);
                }
            }

        }

        public bool GetKeyDown(XboxControllerKey key, int playerIndex = 0)
        {
            if (IsControllerSorting)
                return false;
            
            if (RealIndex.Count <= playerIndex) return false;
            if (RealIndex[playerIndex] < 0) return false;

            var controller = xinputs[RealIndex[playerIndex]];

            return controller != null && TransBtn(controller, key).wasPressedThisFrame;
        }

        public bool GetKeyUp(XboxControllerKey key, int playerIndex = 0)
        {
            if (IsControllerSorting)
                return false;
            
            if (RealIndex.Count <= playerIndex) return false;
            if (RealIndex[playerIndex] < 0) return false;

            var controller = xinputs[RealIndex[playerIndex]];

            return controller != null && TransBtn(controller, key).wasReleasedThisFrame;
        }

        public bool GetKey(XboxControllerKey key, int playerIndex = 0)
        {
            if (IsControllerSorting)
                return false;
            
            if (RealIndex.Count <= playerIndex) return false;
            if (RealIndex[playerIndex] < 0) return false;

            var controller = xinputs[RealIndex[playerIndex]];

            return controller != null && TransBtn(controller, key).isPressed;
        }

        public float GetAxisValue(XboxControllerKey key, int playerIndex = 0)
        {
            if (IsControllerSorting)
                return 0;
            
            if (RealIndex.Count <= playerIndex) return 0;
            if (RealIndex[playerIndex] < 0) return 0;

            var controller = xinputs[RealIndex[playerIndex]];
            if (controller != null)
                return TransBtn(controller, key).ReadValue();
            return 0;
        }

        public float GetHorizontalValue(int playerIndex = 0, bool isLeftStick = true)
        {
            if (IsControllerSorting)
                return 0;
            
            if (RealIndex.Count <= playerIndex) return 0;
            if (RealIndex[playerIndex] < 0) return 0;

            var controller = xinputs[RealIndex[playerIndex]];
            if (controller != null)
                return isLeftStick ? controller.leftStick.x.ReadValue() : controller.rightStick.x.ReadValue();
            return 0;
        }

        public float GetVerticalValue(int playerIndex = 0, bool isLeftStick = true)
        {
            if (IsControllerSorting)
                return 0;
            
            if (RealIndex.Count <= playerIndex) return 0;
            if (RealIndex[playerIndex] < 0) return 0;

            var controller = xinputs[RealIndex[playerIndex]];
            if (controller != null)
                return isLeftStick ? controller.leftStick.y.ReadValue() : controller.rightStick.y.ReadValue();
            return 0;
        }

        private void OnDestroy()
        {
            ClearAllMotorSpeed();
        }


        private void OnApplicationQuit()
        {
            ClearAllMotorSpeed();
        }

        
        public void ClearAllMotorSpeed()
        {
            for (var i = 0; i < RealIndex.Count; i++)
            {
                if (RealIndex[i] > -1)
                {
                    SetMotorSpeeds(0, 0, i);
                }
            }
        }


        /// <summary>
        /// 震动
        /// </summary>
        /// <param name="left">[0-1]</param>
        /// <param name="right">[0-1]</param>
        /// <param name="playerIndex"></param>
        public void SetMotorSpeeds(float left = 0, float right = 0, int playerIndex = 0)
        {

            if (RealIndex.Count <= playerIndex) return;
            if (RealIndex[playerIndex] < 0) return;

            var controller = xinputs[RealIndex[playerIndex]];
            if (controller != null)
                controller.SetMotorSpeeds(left, right);
        }
        
        void _SetMotorSpeeds(float left = 0, float right = 0, int controllerIndex = 0)
        {
            if(xinputs.Count <= controllerIndex) return;
            var controller = xinputs[controllerIndex];
            if (controller != null)
                controller.SetMotorSpeeds(left, right);
        }

        ButtonControl TransBtn(XInputController controller, XboxControllerKey key)
        {
            return key switch
            {
                XboxControllerKey.A => controller.aButton,
                XboxControllerKey.B => controller.bButton,
                XboxControllerKey.X => controller.xButton,
                XboxControllerKey.Y => controller.yButton,
                XboxControllerKey.DpadUp => controller.dpad.up,
                XboxControllerKey.DpadDown => controller.dpad.down,
                XboxControllerKey.DpadLeft => controller.dpad.left,
                XboxControllerKey.DpadRight => controller.dpad.right,
                XboxControllerKey.LUp => controller.leftStick.up,
                XboxControllerKey.LDown => controller.leftStick.down,
                XboxControllerKey.LLeft => controller.leftStick.left,
                XboxControllerKey.LRight => controller.leftStick.right,
                XboxControllerKey.LS => controller.leftStickButton,
                XboxControllerKey.RUp => controller.rightStick.up,
                XboxControllerKey.RDown => controller.rightStick.down,
                XboxControllerKey.RLeft => controller.rightStick.left,
                XboxControllerKey.RRight => controller.rightStick.right,
                XboxControllerKey.RS => controller.rightStickButton,
                XboxControllerKey.LB => controller.leftShoulder,
                XboxControllerKey.RB => controller.rightShoulder,
                XboxControllerKey.LT => controller.leftTrigger,
                XboxControllerKey.RT => controller.rightTrigger,
                XboxControllerKey.Select => controller.selectButton,
                XboxControllerKey.Start => controller.startButton,
                _ => controller.aButton
            };
        }
    }


    public enum XboxControllerKey
    {
        A,
        B,
        X,
        Y,
        DpadUp,
        DpadDown,
        DpadLeft,
        DpadRight,
        LUp,
        LDown,
        LLeft,
        LRight,
        LS,
        RUp,
        RDown,
        RLeft,
        RRight,
        RS,
        LB,
        RB,
        LT,
        RT,
        Start,
        Select,

    }
}
