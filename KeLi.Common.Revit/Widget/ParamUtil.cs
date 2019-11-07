/*
 * MIT License
 *
 * Copyright(c) 2019 kelicto
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

/*
             ,---------------------------------------------------,              ,---------,
        ,----------------------------------------------------------,          ,"        ,"|
      ,"                                                         ,"|        ,"        ,"  |
     +----------------------------------------------------------+  |      ,"        ,"    |
     |  .----------------------------------------------------.  |  |     +---------+      |
     |  | C:\>FILE -INFO                                     |  |  |     | -==----'|      |
     |  |                                                    |  |  |     |         |      |
     |  |                                                    |  |  |/----|`---=    |      |
     |  |              Author: kelicto                       |  |  |     |         |      |
     |  |              Email: kelistudy@163.com              |  |  |     |         |      |
     |  |              Creation Time: 10/30/2019 07:08:41 PM |  |  |     |         |      |
     |  | C:\>_                                              |  |  |     | -==----'|      |
     |  |                                                    |  |  |   ,/|==== ooo |      ;
     |  |                                                    |  |  |  // |(((( [66]|    ,"
     |  `----------------------------------------------------'  |," .;'| |((((     |  ,"
     +----------------------------------------------------------+  ;;  | |         |,"
        /_)_________________________________________________(_/  //'   | +---------+
           ___________________________/___  `,
          /  oooooooooooooooo  .o.  oooo /,   \,"-----------
         / ==ooooooooooooooo==.o.  ooo= //   ,`\--{)B     ,"
        /_==__==========__==_ooo__ooo=_/'   /___________,"
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace KeLi.Common.Revit.Widget
{
    /// <summary>
    /// Parameter utility
    /// </summary>
    public static class ParamUtil
    {
        /// <summary>
        /// Sets the value of the element's parameter.
        /// </summary>
        /// <param name="elm"></param>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        public static void SetValue(this Element elm, string paramName, string value)
        {
            elm.LookupParameter(paramName)?.Set(value);
        }

        /// <summary>
        /// Gets the value of the element's parameter.
        /// </summary>
        /// <param name="elm"></param>
        /// <param name="paramName"></param>
        /// <returns></returns>
        public static string GetValue(this Element elm, string paramName)
        {
            var parameter = elm.LookupParameter(paramName);
            var result = string.Empty;

            switch (parameter.StorageType)
            {
                case StorageType.None:
                    break;

                case StorageType.Integer:
                    result = parameter.AsInteger().ToString();
                    break;

                case StorageType.Double:
                    result = parameter.AsDouble().ToString(CultureInfo.InvariantCulture);
                    break;

                case StorageType.String:
                    result = parameter.AsString();
                    break;

                case StorageType.ElementId:
                    result = parameter.AsElementId().IntegerValue.ToString();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return result;
        }

        /// <summary>
        /// Gets definition by sharing parameter file path.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="paramName"></param>
        /// <param name="canEdit"></param>
        /// <returns></returns>
        public static Definition GetDefinition(this DefinitionGroup group, string paramName, bool canEdit = false)
        {
            var definition = group.Definitions.get_Item(paramName);

            if (definition != null)
                return definition;

            var opt = new ExternalDefinitionCreationOptions(paramName, ParameterType.Text) { UserModifiable = canEdit };

            return group.Definitions.Create(opt);
        }

        /// <summary>
        /// Gets definition groups by sharing parameter file path.
        /// </summary>
        /// <param name="groups"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public static DefinitionGroup GetGroup(this DefinitionGroups groups, string groupName)
        {
            return groups.get_Item(groupName) ?? groups.Create(groupName);
        }

        /// <summary>
        /// Gets definition groups by sharing parameter file path.
        /// </summary>
        /// <param name="uiapp"></param>
        /// <param name="paramPath"></param>
        /// <returns></returns>
        public static DefinitionGroups GetGroups(this UIApplication uiapp, string paramPath)
        {
            if (!File.Exists(paramPath))
                File.CreateText(paramPath);

            uiapp.Application.SharedParametersFilename = paramPath;

            return uiapp.Application.OpenSharedParameterFile()?.Groups;
        }

        /// <summary>
        /// Initializes element or type parameter binding.
        /// </summary>
        /// <param name="uiapp"></param>
        /// <param name="elm"></param>
        /// <param name="paramPath"></param>
        public static void InitParams(this UIApplication uiapp, Element elm, string paramPath)
        {
            var doc = uiapp.ActiveUIDocument.Document;
            var bindingMap = doc.ParameterBindings;
            var gs = uiapp.GetGroups(paramPath);
            var elmCtgs = new CategorySet();

            elmCtgs.Insert(elm.Category);

            foreach (var group in GetGroups(paramPath))
            {
                var paramGroup = gs.GetGroup(group.GroupName);

                foreach (var param in group.Params)
                {
                    var definition = paramGroup.GetDefinition(param.ParamName, param.CanEdit);
                    var binding = bindingMap.get_Item(definition);

                    // If the parameter group's name contains type key, it's means type binding.
                    if (!paramGroup.Name.Contains("Group"))
                    {
                        if (binding is InstanceBinding instanceBinding)
                            bindingMap.ReInsert(definition, instanceBinding);
                        else
                        {
                            instanceBinding = uiapp.Application.Create.NewInstanceBinding(elmCtgs);
                            bindingMap.Insert(definition, instanceBinding);
                        }
                    }
                    else
                    {
                        if (binding is TypeBinding typeBinding)
                            bindingMap.ReInsert(definition, typeBinding);
                        else
                        {
                            typeBinding = uiapp.Application.Create.NewTypeBinding(elmCtgs);
                            bindingMap.Insert(definition, typeBinding);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Getting group list by sharing parameter file path.
        /// </summary>
        /// <returns></returns>
        public static List<Group> GetGroups(string paramPath)
        {
            var texts = File.ReadLines(paramPath).ToList();
            var groups = new List<Group>();
            var paras = new List<Param>();

            foreach (var text in texts)
            {
                var items = text.Split('\t');

                if (items[0] == "GROUP")
                    groups.Add(new Group(items[1], items[2]));

                if (items[0] != "PARAM")
                    continue;

                var param = new Param
                {
                    Guid = items[1],
                    ParamName = items[2],
                    DataType = items[3],
                    DataCatetory = items[4],
                    GroupId = items[5],
                    Visible = Convert.ToBoolean(Convert.ToInt32(items[6])),
                    Description = items[7],
                    CanEdit = Convert.ToBoolean(Convert.ToInt32(items[8]))
                };

                paras.Add(param);
            }

            foreach (var group in groups)
                foreach (var para in paras)
                    if (para.GroupId == group.Id)
                        group.Params.Add(para);

            return groups;
        }

        /// <summary>
        /// Parameter's group
        /// </summary>
        public class Group
        {
            /// <summary>
            /// Initializing parameter's group.
            /// </summary>
            /// <param name="id"></param>
            /// <param name="groupName"></param>
            public Group(string id, string groupName)
            {
                Id = id;
                GroupName = groupName;
                Params = new List<Param>();
            }

            /// <summary>
            /// Returns the id of the parameter's group.
            /// </summary>
            public string Id { get; set; }

            /// <summary>
            /// Returns the name of the parameter's group.
            /// </summary>
            public string GroupName { get; set; }

            /// <summary>
            /// Returns the parameter list of the parameter's group.
            /// </summary>
            public List<Param> Params { get; set; }
        }

        /// <summary>
        /// Element's parameter
        /// </summary>
        public class Param
        {
            /// <summary>
            /// Returns the guid of the element's parameter.
            /// </summary>
            public string Guid { get; set; }

            /// <summary>
            /// Returns the name of the element's parameter.
            /// </summary>
            public string ParamName { get; set; }

            /// <summary>
            /// Returns the data type of the element's parameter.
            /// </summary>
            public string DataType { get; set; }

            /// <summary>
            /// Returns the data category of the element's parameter.
            /// </summary>
            public string DataCatetory { get; set; }

            /// <summary>
            /// Returns the group id of the element's parameter.
            /// </summary>
            public string GroupId { get; set; }

            /// <summary>
            /// Returns the visible of element's parameter.
            /// </summary>
            public bool Visible { get; set; }

            /// <summary>
            /// Returns the description of element's parameter.
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// Returns the can edit of element's parameter.
            /// </summary>
            public bool CanEdit { get; set; }
        }
    }
}