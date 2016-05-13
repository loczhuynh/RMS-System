using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMS.UI.Model
{
    public class CustomerItemOrder
    {
        public CustomerItemOrder()
        {
            _menuItemId = 0;
            _price = 0;
            _itemname = string.Empty;
            _specialRequest = string.Empty;
        }

        public CustomerItemOrder(int menuId, string itemName, decimal price, string specialRequest)
        {
            _menuItemId = menuId;
            _itemname = itemName;
            _price = price;
            _specialRequest = specialRequest;
        }

        //use this property to determine the specific item order
        int _itemOrderId;

        public int ItemOrderId
        {
            get { return _itemOrderId; }
            set { _itemOrderId = value; }
        }


        int _menuItemId;

        public int MenuItemId
        {
            get { return _menuItemId; }
            set { _menuItemId = value; }
        }

        decimal _price;

        public decimal Price
        {
            get { return _price; }
            set { _price = value; }
        }

        string _itemname;

        public string ItemName
        {
            get { return _itemname; }
            set { _itemname = value; }
        }

        string _specialRequest;

        public string SpecialRequest
        {
            get { return _specialRequest; }
            set { _specialRequest = value; }
        }
    }
}
