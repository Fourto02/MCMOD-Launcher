   M S B A M L   `   `   `  ���� B  >updater, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:FourtoMCFourtoMC      FourtoMC.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     (localclr-namespace:FourtoMC     -     � Title$  FourtoMC��$	��300��$	��800�� � WindowStyle$
 None=� � 
ResizeMode$ NoResize=� � WindowStartupLocation$ CenterScreen=� � Icon$ /server-icon.png��.��� .+��� $��/BG.jpg��$��UniformToFill=��� -    ��Name$ texttest��$��	10,10,0,0q�$
��Wrap=�$��	Fourto MC��$��48(�$
��Left=�$	��298��$��64��$	��Top=�$��Noto Sans Thai)�$
��Bold"� � Cursor$
 Hand������  �� EndPoint$ 0.5,1&� �� 
StartPoint$ 0.5,0&�.��� 	 � Color$	 	#FF00FF4A��� $	 	#7F00EDFF��
 � Offset$
 1Q� � Triggers :�  :� RoutedEvent$ TextBlock.Loaded��.[��� .���� .��W�  �� 
TargetName$ texttest�� �� TargetProperty$ (TextBlock.Opacity)� W� From$	 1.0X�	 W� To$	 0.0X� g� Duration$ 0:0:5D� g� AutoReverse .  g� RepeatBehavior$ Forever��]� -    ]�Name$ Downloadinfo_DB����.  $��	0,155,0,0q�$	��113��$	��Top=�$��Center=�$	��768��.+�� -    �Name$ DownloadPercent_PgBar��$��Bottom=�$��45��
����������	#7FFFFFFF � SmallChange$
 0.01Q� � LargeChange$
 0.01Q� � Maximum$ 1Q����� $ 0.5,1&�$ 0.5,0&�.��� $	 	#FF0090FF��� $	 	#7F00FF27��$
 1Q��� 
���   �$��1q�$	��Top=�$��69������ $ 0.5,1&�$ 0.5,0&�.��� $	 	#FF0090FF��� $	 	#7F00FF27��$
 1Q�._�]� ��.  $��	0,0,-2,-2q�.+��� -   $ DownloadStatus_txt��$
��Wrap=�
�������$��Noto Sans Thai)�$��24(����	#00732626$	��164��$,��&กำลังตรวจสอบ
�� �� TextAlignment$ Center=�$��36���� -   $ DownloadPercent_Text��$
��Wrap=�
�������$��Noto Sans Thai)�$��24(����	#00732626$ Right=�$��68��$��37���� $�0���� �� �� $
��Wrap=�
�������$��Noto Sans Thai)�$��24(����	#00732626$��/100%
��$	��100��$��37���� -   $ DownloadSize_txt��$
��Wrap=�
�������$��Noto Sans Thai)�$��24(�$��Right=�$	��170��$��37���� $
��Wrap=�
�������$��Noto Sans Thai)�$��24(����	#00732626$��/��$��Right=�$ Center=�$��17��$��37���� -   $ DownloadingSize_Text��$
��Wrap=�
�������$��Noto Sans Thai)�$��24(����	#00732626$��Right=�$ Right=�$	��170��$��37���� $	��100��$��	0,10,22,0q�$	��Top=�$��Right=�$	��100��$��/server-icon.png���� -	   $ 
Status_txt��$��Center=�$��	0,115,0,0q�$	��Top=�$��40��$	��768��$��24(�$ Right=�$��Status��$��Noto Sans Thai)����	#CCFFFFFF