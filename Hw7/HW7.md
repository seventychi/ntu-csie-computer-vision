# Computer Vision - Homework 7

## 開發環境

- OS: Windows 10 Pro

- Program Language: C# (with .Net Core 3.1)
- IDE: Visual Studio 2019
- Project: Console Application

## 程式說明

程式碼主要寫在 Program.cs，各題目程式皆已實作個別方法，由 Main entry 進行呼叫，答案結果儲存於 answers 資料夾。

各題目程式碼片段、參數及相關演算法說明如下：

(A). Thinning: 

* 先將影像進行二值化處理
* 將二值化影像 DownSampling 成 64 * 64
* 透過迴圈的方式重複執行以下動作
  * 取得 YOKOI 的 Matrix
  * 透過 YOKOI Matrix 取得 Pair Relationship Matrix
  * 透過原始圖片與 Pair Relationship Matrix 的 P 值進行 Connected Shrinking
  * 比對原始圖片與 Shrink 後的結果是否一致，一致則停止迴圈，否則繼續執行迴圈

```c#
var image = GetDownSamplingBitmap(GetBinaryBitmap(new Bitmap("lena.bmp")));

var iteration = 1;

while (true)
{
    // 產生 Yokoi Matrix
    var yokoi = GetYokoiMatrix(image);

    // 透過 Yokoi 產生 Pair Relation Matrix
    var prm = GetPairRelationMatrix(yokoi);

    // 將原始影像與 Pair Relation 的 P 比對後做 Shrinking
    var shrink = GetShrinkImage(image, prm);

    // 比對 Shrink 的結果是否與最後一次的影像相同, 相同則停止進行 Shrinking
    if (Diff(image, shrink))
    {
        break;
    }

    image = shrink;
    image.Save($@"answers\{iteration}.png");

    Console.WriteLine($"iteration: {iteration}");

    iteration += 1;
}
```
## 結果圖片(64 * 64 的放大結果)

<img src="https://raw.githubusercontent.com/seventychi/ntu-csie-computer-vision/main/Hw7/Answers/6.png" style="zoom:500%;" />



