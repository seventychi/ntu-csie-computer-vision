# Computer Vision - Homework 9

## 開發環境

- OS: Windows 10 Pro

- Program Language: C# (with .Net Core 3.1)
- IDE: Visual Studio 2019
- Project: Console Application

## 程式說明

程式碼主要寫在 Program.cs，各題目程式皆已實作個別方法，由 Main entry 進行呼叫，答案結果儲存於 answers 資料夾。

各題目相關演算法說明如下：

(A). Robert's Operator: 

* Threshold: 12
* 遞迴取得所有 x, y 值, 根據每個 x, y 進行處理
* 取得 (x0, y0), (x1, y1) 及 (x1, y0), (x0, y1)
* 根據公式計算 r1, r2
* 藉由公式取得 magnitude 後與 threshold 比較給定新的 pixel 值

(B). Prewitt's Edge Detector

* Threshold: 24
* 遞迴取得所有 x, y 值, 根據每個 x, y 進行處理
* 取得橫向 (x0, y2), (x1, y2), (x2, y2) 及 (x0, y0), (x1, y0), (x2, y0) 及縱向 (x2, y0), (x2, y1), (x2, y2) 及 (x0, y0), (x0, y1), (x0, y2) 的值
* 根據公式計算 p1, p2
* 藉由公式取得 magnitude 後與 threshold 比較給定新的 pixel 值

(C). Sobel's Edge Detector

* Threshold: 38
* 遞迴取得所有 x, y 值, 根據每個 x, y 進行處理
* 取得橫向 (x0, y2), (x1, y2), (x2, y2) 及 (x0, y0), (x1, y0), (x2, y0) 及縱向 (x2, y0), (x2, y1), (x2, y2) 及 (x0, y0), (x0, y1), (x0, y2) 的值
* 根據公式計算 s1, s2
* 藉由公式取得 magnitude 後與 threshold 比較給定新的 pixel 值

(D). Frei and Chen's Gradient Operator

* Threshold: 30
* 遞迴取得所有 x, y 值, 根據每個 x, y 進行處理
* 取得橫向 (x0, y2), (x1, y2), (x2, y2) 及 (x0, y0), (x1, y0), (x2, y0) 及縱向 (x2, y0), (x2, y1), (x2, y2) 及 (x0, y0), (x0, y1), (x0, y2) 的值
* 根據公式計算 s1, s2
* 藉由公式取得 magnitude 後與 threshold 比較給定新的 pixel 值

(E). Kirsch's Compass Operator

* Threshold: 135
* 遞迴取得所有 x, y 值, 根據每個 x, y 進行處理
* 取得公式矩陣的 pixel value
* 計算 k0 ~ k7, 取最大值 kn 作為 magnitude
* 比較 magnitude 與 threshold, 更新 pixel 值

(F). Robinson's Compass Operator

* Threshold: 43
* 遞迴取得所有 x, y 值, 根據每個 x, y 進行處理
* 取得公式矩陣的 pixel value
* 計算 r0 ~ r7, 取最大值 rn 作為 magnitude
* 比較 magnitude 與 threshold, 更新 pixel 值

(G). Nevatia-Babu 5x5 Operator

* Threshold: 12500
* 遞迴取得所有 x, y 值, 根據每個 x, y 進行處理
* 取得公式矩陣的 pixel value
* 計算 n0 ~ n5, 取最大值 nn 作為 magnitude
* 比較 magnitude 與 threshold, 更新 pixel 值

## 結果圖片

|             A. Robert's Operator, Threshold: 12              |          B. Prewitt's Edge Detector, Threshold: 24           |           C. Sobel's Edge Detector, Threshold: 38            |
| :----------------------------------------------------------: | :----------------------------------------------------------: | :----------------------------------------------------------: |
| <img src="https://raw.githubusercontent.com/seventychi/ntu-csie-computer-vision/main/Hw9/Answers/a.png" style="zoom:50%;" /> | <img src="https://raw.githubusercontent.com/seventychi/ntu-csie-computer-vision/main/Hw9/Answers/b.png" style="zoom:50%;" /> | <img src="https://raw.githubusercontent.com/seventychi/ntu-csie-computer-vision/main/Hw9/Answers/c.png" style="zoom:50%;" /> |
|     D. Frei and Chen's Gradient Operator, Threshold: 30      |         E. Kirsch's Compass Operator, Threshold: 135         |        F. Robinson's Compass Operator, Threshold: 43         |
| <img src="https://raw.githubusercontent.com/seventychi/ntu-csie-computer-vision/main/Hw9/Answers/d.png" style="zoom:50%;" /> | <img src="https://raw.githubusercontent.com/seventychi/ntu-csie-computer-vision/main/Hw9/Answers/e.png" style="zoom:50%;" /> | <img src="https://raw.githubusercontent.com/seventychi/ntu-csie-computer-vision/main/Hw9/Answers/f.png" style="zoom:50%;" /> |
|       G.. Nevatia-Babu 5x5 Operator, Threshold: 12500        |                                                              |                                                              |
| <img src="https://raw.githubusercontent.com/seventychi/ntu-csie-computer-vision/main/Hw9/Answers/g.png" style="zoom:50%;" /> |                                                              |                                                              |

