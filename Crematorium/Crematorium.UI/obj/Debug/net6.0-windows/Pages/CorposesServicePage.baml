   M S B A M L   `   `   `  ����I  ECrematorium.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:Crematorium.UI.PagesCrematorium.UI.Pages  5      .    (Crematorium.UI.Pages.CorposesServicePage   5      W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      4local"clr-namespace:Crematorium.UI.Pages     5      
������   � 	Resources  5
       �� MergedDictionaries �� Source�� 5   
    5      �� 5      $; 5pack://application:,,,/Resourse/Styles/BtnStyles.xaml?�6%   �� 5      $< 6pack://application:,,,/Resourse/Styles/TextStyles.xaml?�6%   �� 5      $@ :pack://application:,,,/Resourse/Styles/ElementsStyles.xaml?�6%   �� 5      $@ :pack://application:,,,/Resourse/Styles/DataGridStyles.xaml?�6%   %    5      .��5      ��  �� CornerRadius$ 10��6   ���	#FFF8D0D0._�5   
   �  � ColumnDefinitions 5      �� 5      $��20 �6#   �� 5      �� 5      �� 5      �� 5      �� 5      $��20 �6#    � RowDefinitions 5       �� 5!      $��50 �6    �� 5"      $��20 �6    �� 5#      $��30 �6    �� 5$      $��20 �6    �� 5%      �� 5&      �� 5'      �� 5(      $��20 �6    .+�5,      �� $��0��6   $��6��5-      $��Corpose Service��5.      $��Center=�5/      $��32(�50      �� 53      $��2��6   $��1��54      $��Right=�55      $��Corpose name��56      $��0, 0, 10, 0q�57      �� 58      $��2��6   $��2��59       �� TextAlignment$ Center=�5:      $	��200��5;      $��25��5<      ��5=      �� *InputFindName+�� 5>      $��3��6   $��2��5?      $��80��5@      $
��Left=�5A      ���	#FFF53657$��10, 0, 0, 0q�5C         BaseBtnStyle#��[  5E      ��5D      �� *FindCorposesCommand+.��5F      �� $
��Find��6   5G      &   System.Windows.Controls.DataGrid  5J      -   5          Name$ UsersDataGrid��5L      $��4��5J      $��3��6$   $��1��5K      $��3��6'      
IsReadOnly$
 True��5M        BaseDataGrid#��[ 5N      ��5O      �� *
Corposes+	 �� SelectedItem	 5P      �� 
 �� Path$
 SelectedCorpose�   Columns 5Q      0  *System.Windows.Controls.DataGridTextColumn  5R      ,  &System.Windows.Controls.DataGridColumn   Header$ ID��6)      Width5  /System.Windows.Controls.DataGridLengthConverter$ 80 5S   )   1  +System.Windows.Controls.DataGridBoundColumn   Binding 5T   )   �� $
 Id�  5U      $
 Name��6)   $ * 5V   )    5W   )   �� $

 Name�  5X      $ Surname��6)   $ * 5Y   )    63   �� $
 SurName�  5Z      $ Num passport��6)   $ * 5[   )    63   �� $
 NumPassport�5]      �� 5`      $��4��6   $��4��5a      $��90��5b      ���	#FFF53657#��[  5e      ��5d      �� $
 AddCorposeCommand�.��5f      �� $	��Add��6   $��
0, 0, 0, 0q�5g      5h      �� 5i      $��4��6   $��5��5j      $��90��5k      ���	#FFF53657#��[  5n      ��5m      �� $
 UpdateCorposeCommand�.��5o      �� $
��Edit��6   $��
0, 0, 0, 0q�5p      5q      �� 5r      $��4��6   $��6��5s      $��90��5t      ���	#FFF53657#��[  5w      ��5v      �� $
 DeleteCorposeCommand�.��5x      �� $��Remove��6   $��
0, 0, 0, 0q�5y      5z       � 5}      -   5          �Name$ 	FrameUrns��5}      $��0��5~      $��6��6$   $��0��5      $��8��6!   5�      5�      5�      