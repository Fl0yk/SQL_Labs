   M S B A M L   `   `   `  ����I  ECrematorium.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nulli9clr-namespace:Crematorium.UI.Converters.VisibleConverters+Crematorium.UI.Converters.VisibleConverters  5   	   )    #Crematorium.UI.Pages.ChangeUserPage   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   P
visibility9clr-namespace:Crematorium.UI.Converters.VisibleConverters     5   	   -   5          � WindowStyle$
  None=�5   	   $	��450��5	   	   $	��600��6    � AllowsTransparency$
 True��5
   	   
������  � 	Resources 5       �� MergedDictionaries �� SourceI  CCrematorium.UI.Converters.VisibleConverters.UserOpComboBoxConverter   RoleVisibleL  FCrematorium.UI.Converters.VisibleConverters.UserOpNumPassportConverter  PassportVisibleJ  DCrematorium.UI.Converters.VisibleConverters.UserOpRepeatNumConverter  RepNumVisible�� 5   
    5      �� 5      $; 5pack://application:,,,/Resourse/Styles/BtnStyles.xaml?�6%   �� 5      $< 6pack://application:,,,/Resourse/Styles/TextStyles.xaml?�6%   �� 5      $@ :pack://application:,,,/Resourse/Styles/ComboBoxStyles.xaml?�6%   %W   &	        &	      &	 &       5      61     5      64     5      62   5      .��5      ��  �� CornerRadius$ 10��6   ���	#FFF8D0D0���	#FFB2043E$��3q�5      ._�5   
   �  � ColumnDefinitions 5!      �� 5"      $��20 �6#   �� 5#      $	��160 �6#   �� 5$      $	��280 �6#   �� 5%      $	��120 �6#   �� 5&      $��20 �6#    � RowDefinitions 5(      �� 5)      $��20 �6    �� 5*      $��30 �6    �� 5+      $��* �6    �� 5,      $��40 �6    �� 5-      $��* �6    �� 5.      $��40 �6    �� 5/      $��* �6    �� 50      $��40 �6    �� 51      $��* �6    �� 52      $��40 �6    �� 53      $��* �6    �� 54      $��40 �6    �� 55      $��* �6    �� 56      $��40 �6    �� 57      $��20 �6    .+�5;      �� $��1��6   $��6��5<      $��User creation��5=      $��Center=�5>      $��32(�5?      �� 5B      -   5        $��1��5B      $��1��5C      $��80��5D      $��20��5E      $
��Left=�5F      ���	#FFF53657  BaseBtnStyle#��[ 5I      .��5J      �� $
��Back��6   5K      �� 5L      -   5         ��Name$ OperatinBtn��5Q      $��3��5L      $��13��5M      $	��120��5N      $��35��5O      ���	#FFF53657#��[ 5S      .��5T      �� -   5        	 ��Name$	 	OpBtnName��5T      $��Registration��6/   5U      �� 5X      $��Name:��6   $��1��5Y      $��3��5Z      �� 5[      $��3��6   $��2��5\      $	��260��5]      $��35��5^      ��5_      �� 
 �� Path$

 Name��� 5`      $��Surname:��6   $��1��5a      $��5��5b      �� 5c      $��5��6   $��2��5d      $	��260��5e      $��35��5f      ��5g      �� $
 Surname��� 5h      $��Mail adress:��6   $��1��5i      $��7��5j      �� 5k      $��7��6   $��2��5l      $	��260��5m      $��35��5n      ��5o      �� $
 
MailAdress��� 5p      $��Num passport:��6   $��1��5q      $��9��5r      y�5s      �� $
 	Operation� �� 	Converter# [ �� 5u      $��9��6   $��2��5v      $	��260��5w      $��35��5x      ��5y      �� $
 NumPassport�y�5z      �� $
 	Operation�# [ �� 5}      $��Repeat Num:��6   $��1��5~      $��11��5      y�5�      �� $
 	Operation�# [ �� 5�      $��11��6   $��2��5�      $	��260��5�      $��35��5�      ��5�      �� $
 RepNumPassport�y�5�      �� $
 	Operation�# [ �� 5�      $��Role:��6   $��1��5�      $��11��5�      y�5�      �� $
 	Operation�# [  �� 5�      -   5         ��Name$ RoleComboBox��5�      $��11��5�      $��2��5�      $	��150��5�      $��30��6#   $
��Left=�5�      ���	#FFF53657y�5�      �� $
 	Operation�# [   �� SelectedItem 5�      �� $
 SelectedRole�5�      5�      5�      