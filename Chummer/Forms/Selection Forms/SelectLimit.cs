/*  This file is part of Chummer5a.
 *
 *  Chummer5a is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  Chummer5a is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with Chummer5a.  If not, see <http://www.gnu.org/licenses/>.
 *
 *  You can obtain the full source code for Chummer5a at
 *  https://github.com/chummer5a/chummer5a
 */

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Chummer
{
    public partial class SelectLimit : Form
    {
        private string _strReturnValue = string.Empty;
        private string _strSelectedDisplayLimit = string.Empty;
        private readonly string[] _lstLimits;

        #region Control Events

        public SelectLimit(params string[] lstLimits)
        {
            _lstLimits = lstLimits;
            InitializeComponent();
            this.UpdateLightDarkMode();
            this.TranslateWinForm();
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            string strSelectedLimit = cboLimit.SelectedValue?.ToString();
            if (!string.IsNullOrEmpty(strSelectedLimit))
            {
                _strReturnValue = strSelectedLimit;
                _strSelectedDisplayLimit = ((ListItem)cboLimit.SelectedItem).Name;
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private async void SelectLimit_Load(object sender, EventArgs e)
        {
            // Build the list of Limits.
            using (new FetchSafelyFromSafeObjectPool<List<ListItem>>(Utils.ListItemListPool, out List<ListItem> lstLimitItems))
            {
                foreach (string strLimit in _lstLimits)
                {
                    lstLimitItems.Add(
                        new ListItem(strLimit, await LanguageManager.GetStringAsync("String_Limit" + strLimit + "Short").ConfigureAwait(false)));
                }

                await cboLimit.PopulateWithListItemsAsync(lstLimitItems).ConfigureAwait(false);
                if (lstLimitItems.Count > 1)
                    await cboLimit.DoThreadSafeAsync(x => x.SelectedIndex = 0).ConfigureAwait(false);
                else if (lstLimitItems.Count == 1)
                {
                    string strSelectedLimit = await cboLimit.DoThreadSafeFuncAsync(x => x.SelectedValue?.ToString()).ConfigureAwait(false);
                    if (!string.IsNullOrEmpty(strSelectedLimit))
                    {
                        _strReturnValue = strSelectedLimit;
                        _strSelectedDisplayLimit
                            = ((ListItem) await cboLimit.DoThreadSafeFuncAsync(x => x.SelectedItem).ConfigureAwait(false)).Name;
                        DialogResult = DialogResult.OK;
                        Close();
                    }
                }
                else
                    await cmdOK.DoThreadSafeAsync(x => x.Enabled = false).ConfigureAwait(false);
            }
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        #endregion Control Events

        #region Properties

        /// <summary>
        /// Limit that was selected in the dialogue.
        /// </summary>
        public string SelectedLimit => _strReturnValue;

        /// <summary>
        /// Limit that was selected in the dialogue.
        /// </summary>
        public string SelectedDisplayLimit => _strSelectedDisplayLimit;

        /// <summary>
        /// Description to display on the form.
        /// </summary>
        public string Description
        {
            set => lblDescription.Text = value;
        }

        #endregion Properties
    }
}
