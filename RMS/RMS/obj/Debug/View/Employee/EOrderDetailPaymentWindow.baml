   M S B A M L   `   `   `  ����
  RMS.UI4    .RMS.UI.View.Employee.EOrderDetailPaymentWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  Order Detail Payment��5   	   $��600.57��6&   $��881.424��66    � WindowStartupLocation$ CenterScreen=�6F   .��5      �  J� RenderTransformOrigin$ 	0.5,0.523&�6    � RowDefinitions 5   
   �� 5      $��295.2 �6   �� 5      $
��95.2 �6   �� 5	      $��205.6 �6    � ColumnDefinitions 5   
   �� 5      $��108.8 �6   �� 5      $��648.8 �6   �� 5      $��541.6 �6   .+�5   
   &   System.Windows.Controls.DataGrid  -   5        $��gridOrdersDetail��5      $	��290��5      $	��Top=�5   -   $��1��6E   $��0��6U      AutoGenerateColumns .     AlternatingRowBackground
 �/��� �� AlternationCount$ 2��5   b      CanUserAddRows .  	   ColumnWidth5  /System.Windows.Controls.DataGridLengthConverter$	 * 6�   $��2��6�   $��0.2,0,541.2,0q�6�   ��5   b   �� *OpenMenuOrders+
   Columns
 5      4  .System.Windows.Controls.DataGridTemplateColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$ Order Id��6)      
IsReadOnly .    Width$ 60 6M      CellTemplate 5      �� 5      .R�5      �� ��6(   �� *	idOrder+5      5        5      $ 	Item Name��6)    . $	 250 6N    5      �� 5      .R�5      �� ��6(   �� *
ItemName+5      5!        5"      $ Price��6)    . $	 150 6J    5#      �� 5$      .R�5%      �� ��6(   �� *Price+ �� StringFormat$ c��5&      5(      5+      �� 5,   
   $��1��6   $��2��6%   $��0.2,295,541.2,123q�69   $��3��6T   .+�5-      �� -   5        $��txtTotalPrice��5-       �� TextAlignment$ Right=�6-   Total Price:6R   �� 5.      -   5        $��txtTax��5.      $ Right=�6&   Tax:6B   �� 5/      -   5        $��txtTip��5/      $ Right=�6&   Tip:6B   �� 50      $ Right=�6   )'---------------------------------------6W   �� 51      -   5        $��txtSubTotal��51      $ Right=�6+   
Sub Total:6M   52      �� 53   
   -   5        $��btnBack��53      $
��Back��6    $��0��6/   $��2��6?   $��2��6L   $��0,103.6,648.6,25q�6`   6�   �� 54   
   -   5        $��btnPay��54      $	��Pay��6   $��2��6-   $��2��6=   $��0.4,103.6,424.2,25q�6J   6f   �� 55   
   -   5        $��txtLeaveTip��55      $��1��6%   $��2��65   $��259.2,46.6,75.6,101q�6B   $��18(�6_   $
��Bold"�6m    �� TextAlignment$ Justify=�6   �� 56   
   -	   5        $��btnLeaveTip��56      $��	Leave Tip��6$   $��1��68   $��2��6H   $��84.2,48.6,418.6,99q�6U   #��� ��6q   $
��Bold"�6�   $��14(�6�   6�   57      58      