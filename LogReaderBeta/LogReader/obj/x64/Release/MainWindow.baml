   M S B A M L   `   `   `  ���� D  @LogReader, Version=0.4.5.0, Culture=neutral, PublicKeyToken=null%clr-namespace:LogReader	LogReader      LogReader.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     )localclr-namespace:LogReader     b ^MaterialDesignThemes.Wpf, Version=4.1.0.2556, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes     MaterialDesignBody#���     MaterialDesignPaper#���  $��Medium"�$��14(�  � Title$  LogReader V 0.4.5��$	��800��$
��1600�� � Icon$ Cursor1.cur�� � Cursor$ Cursor1.cur��$	��800��$
��1600��$	��800��$
��1600����:  4MaterialDesignThemes.Wpf.MaterialDesignFontExtension  .��� .+��� -    ��Name$ SGTINButton��$ Cursor1.cur��$��Поиск по SGTIN��$
��Left=�$��
18,263,0,0q�$	��Top=�$	��139�� J� IsManipulationEnabled . ���	#FF94C646���	#DD000000�� -    ��Name$ EnterBox��$ Cursor1.cur����. $��Visible=�$
��Left=�$	��117��$��
18,131,0,0q� �� TextWrapping$
 Wrap=� �� AcceptsReturn . " �� VerticalScrollBarVisibility$ Visible=�$	��Top=�$	��472��4  .System.Windows.Automation.AutomationProperties	   Name$	 SGTINNumber�����	#FF673AB7���	#DD000000���	#FF94B95A
   HelpText$!
 Искомые данные���� -   $ 	ResultBox��$ Cursor1.cur����. $��Visible=�$
��Left=�$	��404��$��
10,355,0,0q�$
 Wrap=�$	��Top=�$	��938�����	#FF673AB7
���   ��� -   $ 
SaveButton��$ Cursor1.cur��$$��Сохранить в файл��$
��Left=�$��338,263,0,0q�$	��Top=�$	��152��$��32�����	#FF94C646���	#DD000000�� $��0q�$��Logo_web_2.png��$
��Fill=�$	��Top=�$	��110���� -    ��Name$ Listobox��$��953,131,311,10q�$ Cursor1.cur������ .R��� .+��� -   $ Cursor1.cur����. $��Visible=� . ���	#FF94C646���	#DD000000���� *
SRIDNumber+�� -   $ 
SRIDButton��$ Cursor1.cur��$��Поиск по SRID��$
��Left=�$��182,263,0,0q�$	��Top=�$	��130�����	#FF94C646���	#DD040404�� -   $ Listobox_Bad��$��1268,131,12,10q����� .R��� .+��� -	   $ Cursor1.cur����. $��Visible=� . ���	#FF673AB7���	#DD000000���� *SGTINNumber+�� -
    ��Name$ Upload��$ Cursor1.cur��$
��Left=�$	��208��$��719,131,0,0q�$	��Top=�$	��211��$��
upload.png�� J� 	AllowDrop . �� -   $ TechPassButton��$ Cursor1.cur��$-��'Пароль тех. поддержки��$
��Left=�$��506,131,0,0q�$	��Top=�$	��208��$��33�����	#FF94C646���	#DD000000� -    �Name$ pbStatus��$ Cursor1.cur��$
��Left=�$��24��$��
18,315,0,0q�$	��Top=�$	��472�����	#FFB39DDB�� -   $	 AKB��$	��AKB��$ Cursor1.cur��$
��Left=�$��506,169,0,0q�$	��Top=�$	��105�����	#FF94C646���	#DD000000�� -   $ MB��$��	MB Errors��$ Cursor1.cur��$
��Left=�$��616,169,0,0q�$	��Top=�$��98��
���   ����	#FF94C646�� -   $ MK��$��	MK Errors��$ Cursor1.cur��$
��Left=�$��616,206,0,0q�$	��Top=�$��98��
���   ����	#FF94C646�� -   $ 	Sattelite��$��Спутники��$ Cursor1.cur��$
��Left=�$��507,206,0,0q�$	��Top=�$	��104�����	#FF94C646���	#DD000000