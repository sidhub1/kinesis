﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Shapes;
using KineSis.Utils;
using KineSis.Profiles;

namespace KineSis.UserInterface.Entities.Groups {
    class Pages : Group {
        private static List<Group> groups = new List<Group>();
        private int pos = 0;
        private Boolean posChanged = false;
        private Boolean preSelected = false;
        static readonly Pages instance = new Pages();

        static Pages() {
        }

        Pages() {
        }

        public static Pages Instance {
            get {
                return instance;
            }
        }

        String Group.Name {
            get {
                return "pages";
            }
        }

        private static List<Element> elements;

        public static List<Element> Elements {
            get {
                return elements;
            }

            set {
                elements = value;
            }
        }

        public static List<Group> Groups {
            get {
                if (groups.Count == 0) {
                    Group navigate = new Navigate(Pages.Instance);
                    groups.Add(navigate);
                    Group scroll = new Scroll(Pages.Instance);
                    groups.Add(scroll);
                    Group select = new Select(Pages.Instance);
                    groups.Add(select);
                    Group main = UIManager.MainGroup;
                    groups.Add(main);
                }
                return groups;
            }
        }

        void Group.Draw(Canvas c) {

            if (UIManager.SecondHand != null && UIManager.FirstHandNumber != 0 && UIManager.FirstHand.IsSelected) {

                double centerX = ( UIManager.FirstHand.X + UIManager.SecondHand.X ) / 2;
                double centerY = ( UIManager.FirstHand.Y + UIManager.SecondHand.Y ) / 2;

                Double delta = Math.Abs(UIManager.Delta) + 1;

                Double centerDiameter =Math.Sqrt(Math.Pow(UIManager.FirstHand.X - UIManager.SecondHand.X, 2) + Math.Pow(UIManager.FirstHand.Y - UIManager.SecondHand.Y, 2)) - 150;

                if (centerDiameter > 2 * (1.75*delta - 225)) {
                    centerDiameter = 2 * (1.75*delta - 225);
                } 
                if (centerDiameter <= 0) {
                    centerDiameter = 1;
                }

                double rightAreaX = UIManager.RightHand.X;
                double rightAreaY = UIManager.LeftHand.Y;

                if (rightAreaX > centerX + 1.75 * delta - 150 ) {
                    rightAreaX = centerX + 1.75 * delta - 150;
                }

                double rightSelectionX = centerX + 1.75*delta;
                double rightSelectionY = centerY;

                double leftSelectionX = centerX - 1.75 * delta;
                double leftSelectionY = centerY;

                double leftAreaX = UIManager.LeftHand.X;
                double leftAreaY = UIManager.RightHand.Y;

                if (leftAreaX < centerX - 1.75 * delta + 150) {
                    leftAreaX = centerX - 1.75 * delta + 150;
                }

                if (UIManager.LeftHand.X < leftSelectionX + 75 && UIManager.LeftHand.Y < leftSelectionY + 75 && UIManager.LeftHand.Y > leftSelectionY - 75) {
                    preSelected = true;
                } else if (UIManager.LeftHand.X > leftSelectionX + 150 && UIManager.LeftHand.Y < leftSelectionY + 75 && UIManager.LeftHand.Y > leftSelectionY - 75) {
                    if (preSelected) {
                        UIManager.SelectedGroup = Groups[pos];
                        preSelected = false;
                        pos = 0;
                        posChanged = false;
                    }
                } else if (preSelected && (UIManager.LeftHand.Y >= leftSelectionY + 75 || UIManager.LeftHand.Y <= leftSelectionY - 75)) {
                    preSelected = false;
                }

                if (UIManager.LeftHand.Y > leftAreaY - 75 && UIManager.LeftHand.Y < leftAreaY + 75) {
                    posChanged = false;
                } else if (UIManager.LeftHand.Y <= leftAreaY - 75 && !preSelected) {
                    IncrementPos();
                    posChanged = true;
                } else if (UIManager.LeftHand.Y >= leftAreaY + 75 && !preSelected) {
                    DecrementPos();
                    posChanged = true;
                }

                String caption = null;
                if (GetSubmenu().Equals(UIManager.MainGroup.Name)) {
                    caption = "back 2 " + GetSubmenu();
                }

                CanvasUtil.DrawSubmenu(c, GetSubmenu(), caption, true, null, centerX, centerY, centerDiameter, 150, leftAreaX, leftAreaY, rightAreaX, rightAreaY, leftSelectionX, leftSelectionY, rightSelectionX, rightSelectionY, preSelected);
            }
        }

        private void IncrementPos() {
            if (pos < Groups.Count - 1 && !posChanged) {
                pos++;
            } else if (pos == Groups.Count - 1 && !posChanged) {
                pos = 0;
            }
        }

        private void DecrementPos() {
            if (pos > 0 && !posChanged) {
                pos--;
            } else if (pos == 0 && !posChanged) {
                pos = Groups.Count - 1;
            }
        }

        private String GetSubmenu() {
            return Groups[pos].Name;
        }
    }
}
