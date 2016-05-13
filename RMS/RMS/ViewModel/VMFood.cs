using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System;
using System.Windows;
using RMS.Server.BL;
using RMS.UI.Model;

namespace RMS.UI.ViewModel
{
    class VMFood : VMBase
    {
        #region Variables
        IList<MenuOrderBL> currentOrder;
        IList<MenuOrderBL> submittedOrder;
        CategoryBL[] currentMenuCategories;
        List<IList<MenuItemBL>> currentMenu;

        List<MenuItemBL> drinksInOrder;
        int drinkCategory;
        #endregion // Variables

        #region Constructors
        public VMFood(string displayName) : base(displayName)
        {
            currentMenu = new List<IList<MenuItemBL>>();
            drinksInOrder = new List<MenuItemBL>();
        }
        #endregion // Constructors

        #region Properties
        public List<MenuItemBL> DrinksInOrder
        {
            get { return drinksInOrder; }
        }

        public int DrinkCategory
        {
            get { return drinkCategory; }
            set { drinkCategory = value; }
        }
       #endregion // Properties

        #region Commands
        #endregion // Commands

        #region Methods
        public void UpdateDrinksInOrder()
        {
            int i = 0;
            int j = 0;
            DrinksInOrder.Clear();

            // This algorithm sucks...
            if(submittedOrder == null) { return; }
            while(i < submittedOrder.Count)
            {
                j = submittedOrder[i].idMenuItem;
                foreach(MenuItemBL temp in currentMenu)
                {
                    if (temp.idMenuItem == DrinkCategory && temp.category == "Drink")
                    {
                        DrinksInOrder.Add(temp);
                    }
                }

                i += 1;
            }
        }
        #endregion // Methods
    }
}
