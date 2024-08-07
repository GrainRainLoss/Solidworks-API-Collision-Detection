# SolidWorks-API-Collision-Detection
Use SolidWorks API to MovePart and Collision Detection   利用solidworks的API来移动控件物体以及进行碰撞检测

# visual studio 2022 利用Nuget 安装这些库
![屏幕截图 2024-08-07 141342](https://github.com/user-attachments/assets/5fd6c706-3f0a-40ef-acfe-d368b8f78d13)


# 打开solidworks
![屏幕截图 2024-08-07 141552](https://github.com/user-attachments/assets/18a7da0d-a5f4-46ba-9588-aa987a4b792b)

可以看到有两个控件
# 部件运动
使用封装的函数void MovePart(string partName,double thetax,double thetay,double thetaz, double xDistance, double yDistance, double zDistance,double scale)改变部件
partname 是部件名称 比如 床 ![屏幕截图 2024-08-07 141756](https://github.com/user-attachments/assets/d1bc08b9-eee5-4537-91a8-f998966a5623)

他的部件名应该为string partName = "bed2-1"; 您可以参考样例来学习更多的使用；
thetax、thetay、thetaz分别是让物块按照不同的轴转动，您也可以三个都写进行复杂转动，该函数中已经将多角度转动融合了。注意，这里的转动是转动到指定位置，而不是转动多少度。比如你想要30°，60°，您应该直接写对应的角度，而不是运行两次30°。
xDistance、yDistance、zDistance分别是指定轴方向的平移，这个平移经不确定的测量，可能1代表1m的运动距离 这个就是实打实的运动1m，比如你要0，1，2，3m的位置距离，那就多次写入运行1
scale是放缩该部件，这里不做赘述。
# 连接solidworks 
打开solidworks后 连接solidworks 这里我写了一个简单的界面的连接按钮。

![屏幕截图 2024-08-07 142742](https://github.com/user-attachments/assets/72ed5d46-108e-4adc-8fe6-c05b55be0460)

连接成功 

![屏幕截图 2024-08-07 142704](https://github.com/user-attachments/assets/6dc80dfe-6e9a-4476-a8c5-67905fe08ed6)

即可对物块进行操作了，例如 旋转30° 

![屏幕截图 2024-08-07 142854](https://github.com/user-attachments/assets/190aa291-b518-498a-8d0d-4fd72354c0b5)


# 进行碰撞检测
bool CheckForInterference() 调用后如果当前所有界面中有碰撞 那么会返回true 否则返回false
# 组合运用部件运动和碰撞检测
组合运动上述两个功能，即可用solidworks模拟机械设备运动，并知晓当前位置是否会发生碰撞，最终知道在各个位置是否会出现碰撞冲突，存入数据库，以指导机械运动的安全联锁。
