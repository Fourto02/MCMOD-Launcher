   M S B A M L   `   `   `  ���� C  ?FourtoMC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:FourtoMCFourtoMC      FourtoMC.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     (localclr-namespace:FourtoMC     -     � Title$  FourtoMC��$	��720��$
��1280�� � WindowStyle$
 None=� � 
ResizeMode$ NoResize=� � WindowStartupLocation$ CenterScreen=� � Icon$ /server-icon.ico��.��� .+��� $��/BG.jpg��$
��Fill=��� -    ��Name$ 
PlayButton��$��Download��$��863,573,35,34q�
�������$��24(� �� 	IsDefault . ����  �� 
TargetType" ��.���  �� Property � ��	 �� Value	 �� 
 �� EndPoint$
 0.5,1&� �� 
StartPoint$ 0.5,0&�.���  � Color$ 	#FF0090FF��� $ 	#7F00FF27�� � Offset$ 1Q���  � ��	 ��  �� 
TargetType" ��.R��� #��z��._��� $��Center=�$��Center=� �� Triggers P�  P� Property �@J�IsMouseOver P� Value . . ���  � ��	 �� $
 0.5,1&�$ 0.5,0&�.��� $ 	#FFC7E7FF��� $ 	#7FABFF9A��$ 1Q��� -    ��Name$ texttest��$��	10,10,0,0q�$
��Wrap=�$��	Fourto MC��$��48(�$
��Left=�$	��298��$��64��$	��Top=�$��Noto Sans Thai)�$
��Bold"� � Cursor$
 Hand������ $
 0.5,1&�$ 0.5,0&�.��� $ 	#FF00FF4A��� $ 	#7F00EDFF��$ 1Q� � Triggers :�  :� RoutedEvent$ TextBlock.Loaded��.[��� .���� .��W�  �� 
TargetName$ texttest�� �� TargetProperty$ (TextBlock.Opacity)� W� From$	 1.0X�	 W� To$	 0.0X� g� Duration$ 0:0:5D� g� AutoReverse .  g� RepeatBehavior$ Forever��]� ��.  $��1054,10,0,657q�$
��Left=�$	��216��.+��� -   $ Close_bt��$��X��$��Right=�$��58��$��1,1,1,1q�
�������$��20(����	#00000000$��SAO UI)����� " ��.���  � ��	 �� $
 0.5,1&�$ 0.5,0&�.��� $ 	#FF0090FF��� $ 	#7F00FF27��$ 1Q���  � ��	 �� " ��.R��� #��z��._��� $��Center=�$��Center=� P�  �@J�IsMouseOver . . ���  � ��	 �� $
 0.5,1&�$ 0.5,0&�.��� $ 	#FFC7E7FF��� $ 	#7FABFF9A��$ 1Q��� -   $ minisized_bt��$��V��$��Right=�$��1,1,1,1q�
�������$��20(����	#00000000$��58��$��SAO UI)�$��0,0,5,0q����� " ��.���  � ��	 �� $
 0.5,1&�$ 0.5,0&�.��� $ 	#FF0090FF��� $ 	#7F00FF27��$ 1Q���  � ��	 �� " ��.R��� #��z��._��� $��Center=�$��Center=� P�  �@J�IsMouseOver . . ���  � ��	 �� $
 0.5,1&�$ 0.5,0&�.��� $ 	#FFC7E7FF��� $ 	#7FABFF9A��$ 1Q�]� -    ]�Name$ Downloadinfo_DB����.  $��49,573,462,34q�.+�� -    �Name$ DownloadPercent_PgBar��$��Bottom=�$��45��
����������	#7FFFFFFF � SmallChange$
 0.01Q�  � LargeChange$
  0.01Q�! � Maximum$! 1Q����� $
 0.5,1&�$ 0.5,0&�.��� $ 	#FF0090FF��� $ 	#7F00FF27��$ 1Q��� 
���   �$��1q�$	��Top=�$��69������ $
 0.5,1&�$ 0.5,0&�.��� $ 	#FF0090FF��� $ 	#7F00FF27��$ 1Q�._�]� ��.  $��	0,0,-2,-2q�.+��� -   $ DownloadStatus_txt��$
��Wrap=�
�������$��Noto Sans Thai)�$��24(����	#00732626$	��164��$,��&กำลังตรวจสอบ
��" �� TextAlignment$" Center=�$��36���� -	   $ DownloadPercent_Text��$
��Wrap=�
�������$��Noto Sans Thai)�$��24(����	#00732626$" Right=�$��68��$��37���� $�0���� �� �� $
��Wrap=�
�������$��Noto Sans Thai)�$��24(����	#00732626$��/100%
��$	��100��$��37���� -
   $ DownloadSize_txt��$
��Wrap=�
�������$��Noto Sans Thai)�$��24(�$��Right=�$	��170��$��37���� $
��Wrap=�
�������$��Noto Sans Thai)�$��24(����	#00732626$��/��$��Right=�$" Center=�$��17��$��37���� -   $ DownloadingSize_Text��$
��Wrap=�
�������$��Noto Sans Thai)�$��24(����	#00732626$��Right=�$" Right=�$	��170��$��37���� 
���   �$��1q�$��49,496,759,147q����� $
 0.5,1&�$ 0.5,0&�.��� $ 	#7FFF97AA��� $ 	#FF0C9AD9��$ 1Q�._�]� ��.  $��	0,0,-1,-2q�.+��� $
��Wrap=�$��ชื่อ��$��24(�$" Center=�$��Noto Sans Thai)�# J� RenderTransformOrigin$# 0.507,0.649&�$	��119��$��32�����	#FFFFD8D8�� -   $ ��Name$$ Name_edt��$��Right=�$��Noto Sans Thai)����	#AE000000% �� SelectionBrush% �	#B2E0E0E0& �� SelectionOpacity$& 1Q�$��24(�
�������$	��345��$
��Bold"�' �� TextWrapping$
' Wrap=�( �� 	MaxLength$( 13��) �� 
IsReadOnly) .  $��Italic$�$��36��$��NoName��$
 Hand��4  .System.Windows.Automation.AutomationProperties*   HelpText$
* Name��