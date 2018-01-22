//预览
function Preview() {
    document.all.factory.printing.Preview();
}

function Print() {
    document.all.factory.printing.Print(true); //无确认打印，true时打印前需进行确认
}

function PrintSetup() {
    document.all.factory.printing.PageSetup();
}

function SetPrintSettings() {
    // -------------------基本功能，可免费使用-----------------------
    document.all.factory.printing.header = ""; //页眉
    document.all.factory.printing.footer = ""; //页脚
    ////document.all.factory.printing.SetMarginMeasure(1); //页边距单位，1为毫米，2为英寸//边距设置，需要注意大部分打印机都不能进行零边距打印，即有一个边距的最小值，一般都是6毫米以上
    //设置边距的时候时候如果设置为零，就会自动调整为它的最小边距
    document.all.factory.printing.leftMargin = 5; //左边距
    document.all.factory.printing.topMargin = 5; //上边距
    document.all.factory.printing.rightMargin = 5; //右边距
    document.all.factory.printing.bottomMargin = 5; //下边距
    document.all.factory.printing.portrait = true; //是否纵向打印，横向打印为false
    //--------------------高级功能---------------------------------------------
    //document.all.factory.printing.printer = "EPSON LQ-1600KIII"; //指定使用的打印机
    //document.all.factory.printing.printer = "\\\\cosa-data\\HPLaserJ";//如为网络打印机，则需要进行字符转义
    ////document.all.factory.printing.paperSize = "A4"; //指定使用的纸张
    document.all.factory.printing.paperSource = "Manual feed"; //进纸方式，这里是手动进纸
    ////document.all.factory.printing.copies = 1; //打印份数
    ////document.all.factory.printing.printBackground = false; //是否打印背景图片
    ////document.all.factory.printing.SetPageRange(false, 1, 3); //打印1至3页
    //---------------------常用函数--------------------------------
    //document.all.factory.printing.Print(false); //无确认打印，true时打印前需进行确认
    //document.all.factory.printing.PrintSetup(); //打印设置
    //document.all.factory.printing.Preview(); //打印预览
    //document.all.factory.printing.WaitForSpoolingComplete(); //等待上一个打印任务完全送入打印池，在连续无确认打印时非常有用
    //document.all.factory.printing.EnumPrinters(index); //枚举已安装的所有打印机，主要用于生成打印机选择功能
}  