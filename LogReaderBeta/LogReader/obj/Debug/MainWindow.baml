   M S B A M L   `   `   `  ����D  @LogReader, Version=0.4.1.0, Culture=neutral, PublicKeyToken=null%clr-namespace:LogReader	LogReader  5   	       LogReader.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   )localclr-namespace:LogReader     5   	   b ^MaterialDesignThemes.Wpf, Version=4.1.0.2556, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5	   	      MaterialDesignBody#���   5
   	     MaterialDesignPaper#���  5   	   $��Medium"�5   	   $��14(�5   	     � Title$  LogReader V 0.4.2��5   	   $	��800��6#   $
��1600��60    � Icon$ Cursor1.cur��6=    � Cursor$ Cursor1.cur��6P   $	��800��6e   $
��1600��6u   $	��800��6�   $
��1600��6�   ��5   	   :  4MaterialDesignThemes.Wpf.MaterialDesignFontExtension  .��5      � .+�5   
   �� -   5         ��Name$ SGTINButton��5      $ Cursor1.cur��6&   $��Поиск по SGTIN��6;   $
��Left=�6T   $��
18,263,0,0q�6o   $	��Top=�6�   $	��139��6�    J� IsManipulationEnabled . ���	#FF94C646���	#DD0000006�   �� 5   
   -   5         ��Name$ EnterBox��5      $ Cursor1.cur��6$   ��. $��Visible=�6^   $
��Left=�6�   $	��117��6�   $��
18,131,0,0q�6�    �� TextWrapping$
 Wrap=�6�    �� AcceptsReturn . " �� VerticalScrollBarVisibility$ Visible=�6�   $	��Top=�6  $	��472��64  4  .System.Windows.Automation.AutomationProperties	   Name$	 SGTINNumber��6@  ���	#FF673AB7���	#DD000000���	#FF94B95A
   HelpText$!
 Искомые данные��6�  �� 5   
   -   5        $ 	ResultBox��5      $ Cursor1.cur��6%   ��. $��Visible=�6_   $
��Left=�6�   $	��404��6�   $��
10,355,0,0q�6�   $
 Wrap=�6�   $	��Top=�6�   $	��938��6�   ���	#FF673AB7
���   �6  �� 5   
   -   5        $ 
SaveButton��5      $ Cursor1.cur��6%   $$��Сохранить в файл��6:   $
��Left=�6U   $��338,263,0,0q�6p   $	��Top=�6�   $	��152��6�   $��32��6�   ���	#FF94C646���	#DD0000006�   �� 5   
   $��0q�6   $��Logo_web_2.png��6   $
��Fill=�63   $	��Top=�6B   $	��110��6Z   �� 5   
   -   5         ��Name$ Listobox��5      $��953,131,311,10q�6$   $ Cursor1.cur��6<   ��5      �� 5      .R�5      �� .+�5      �� -   5        $ Cursor1.cur��5   A   ��. $��Visible=�6{    . ���	#FF94C646���	#DD000000��6!   �� *
SRIDNumber+6�   5      5      5      �� 5    
   -   5        $ 
SRIDButton��5       $ Cursor1.cur��6%   $��Поиск по SRID��6:   $
��Left=�6R   $��182,263,0,0q�6m   $	��Top=�6�   $	��130��6�   ���	#FF94C646���	#DD0404046�   �� 5!   
   -   5        $ Listobox_Bad��5!      $��1268,131,12,10q�6(   ��5"      �� 5#      .R�5$      �� .+�5%      �� -	   5        $ Cursor1.cur��5%   B   ��. $��Visible=�6|    . ���	#FF673AB7���	#DD000000��6!   �� *SGTINNumber+6�   5&      5'      5)      �� 5*   
   -
   5         ��Name$ Upload��5*      $ Cursor1.cur��6    $
��Left=�65   $	��208��6P   $��719,131,0,0q�6]   $	��Top=�6r   $	��211��6�   $��
upload.png��6�    J� 	AllowDrop . 6�   �� 5+   
   -   5        $ TechPassButton��5+      $ Cursor1.cur��6)   $-��'Пароль тех. поддержки��6>   $
��Left=�6^   $��506,131,0,0q�6y   $	��Top=�6�   $	��208��6�   $��33��6�   ���	#FF94C646���	#DD0000006�   � 5,   
   -   5         �Name$ pbStatus��5,      $ Cursor1.cur��6(   $
��Left=�6=   $��24��6X   $��
18,315,0,0q�6d   $	��Top=�6x   $	��472��6�   ���	#FFB39DDB6�   �� 5-   
   -   5        $	 AKB��5-      $	��AKB��6   $ Cursor1.cur��6,   $
��Left=�6A   $��506,169,0,0q�6\   $	��Top=�6q   $	��105��6�   ���	#FF94C646���	#DD0000006�   �� 5.   
   -   5        $ MB��5.      $��	MB Errors��6   $ Cursor1.cur��61   $
��Left=�6F   $��616,169,0,0q�6a   $	��Top=�6v   $��98��6�   
���   ����	#FF94C6466�   �� 5/   
   -   5        $ MK��5/      $��	MK Errors��6   $ Cursor1.cur��61   $
��Left=�6F   $��616,206,0,0q�6a   $	��Top=�6v   $��98��6�   
���   ����	#FF94C6466�   �� 50   
   -   5        $ 	Sattelite��50      $��Спутники��6$   $ Cursor1.cur��67   $
��Left=�6L   $��507,206,0,0q�6g   $	��Top=�6|   $	��104��6�   ���	#FF94C646���	#DD0000006�   51      52      