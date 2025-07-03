using PCB_Investigator.Automation.DBFilter;
using PCB_Investigator.Automation.DesignHistory;
using PCB_Investigator.PCBIWindows;
using PCBI.Automation;
using PCBI.MathUtils;
using PCBI.Plugin.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PCB_Investigator_API_Examples
{
/// <summary>
/// This is example code to show ways to use the PCB-Investigator API
/// </summary>
    private static partial class PCB_Investigator_API_Example_Class
    {
        /// <summary>
        /// Example method to generate a component list Excel report by using the PCB-Investigator API
        /// </summary>
        private static string Example_GenerateComponentListExcelReport(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken, string initialDir, string filter, int filterIndex, string fileName, string sheetName, string loadedDesignPathOrServer, PCBI.Plugin.Interfaces.FileLocation fileLocation, FileType fileType, string defaultFileName)
        {
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
            List<List<PCBI.Automation.IO.Excel.Cell>> cellList = new List<List<PCBI.Automation.IO.Excel.Cell>>();
            List<PCBI.Automation.IO.Excel.Cell> headerRow = new List<PCBI.Automation.IO.Excel.Cell>
            {
                new PCBI.Automation.IO.Excel.Cell() { Value = "Reference", Type = PCBI.Automation.IO.Excel.Cell.CellType.STRING },
                new PCBI.Automation.IO.Excel.Cell() { Value = "Package name", Type = PCBI.Automation.IO.Excel.Cell.CellType.STRING },
                new PCBI.Automation.IO.Excel.Cell() { Value = "Part number", Type = PCBI.Automation.IO.Excel.Cell.CellType.STRING },
                new PCBI.Automation.IO.Excel.Cell() { Value = "X Position (mm)", Type = PCBI.Automation.IO.Excel.Cell.CellType.STRING },
                new PCBI.Automation.IO.Excel.Cell() { Value = "Y Position (mm)", Type = PCBI.Automation.IO.Excel.Cell.CellType.STRING },
                new PCBI.Automation.IO.Excel.Cell() { Value = "Rotation", Type = PCBI.Automation.IO.Excel.Cell.CellType.STRING },
                new PCBI.Automation.IO.Excel.Cell() { Value = "Mounting Type", Type = PCBI.Automation.IO.Excel.Cell.CellType.STRING },
                new PCBI.Automation.IO.Excel.Cell() { Value = "Populated", Type = PCBI.Automation.IO.Excel.Cell.CellType.STRING }
            };
            cellList.Add(headerRow);
            foreach (ICMPObject cmp in step.GetAllCMPObjects().OrderBy(c => c.Ref))
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested)
                    return "Operation was cancelled.";
        
                // Get the position of the component
                PointD positionMils = cmp.GetPosition(); //always in mils
        
                // Check if the component is populated
                bool isPopulated = IAttribute.GetStandardAttribute(cmp, PCBI.FeatureAttributeEnum.comp_ignore) == null && IAttribute.GetStandardAttribute(cmp, PCBI.FeatureAttributeEnum.no_pop) == null;
                string compType = IAttribute.GetStandardAttribute(cmp, PCBI.FeatureAttributeEnum.comp_mount_type)?.Value?.ToString() ?? "Unknown";
                List<PCBI.Automation.IO.Excel.Cell> row = new List<PCBI.Automation.IO.Excel.Cell>
                {
                    new PCBI.Automation.IO.Excel.Cell() { Value = cmp.Ref, Type = PCBI.Automation.IO.Excel.Cell.CellType.STRING },
                    new PCBI.Automation.IO.Excel.Cell() { Value = cmp.UsedPackageName, Type = PCBI.Automation.IO.Excel.Cell.CellType.STRING },
                    new PCBI.Automation.IO.Excel.Cell() { Value = cmp.PartName, Type = PCBI.Automation.IO.Excel.Cell.CellType.STRING },
                    new PCBI.Automation.IO.Excel.Cell() { Value = IMath.Mils2MM(positionMils.X), Type = PCBI.Automation.IO.Excel.Cell.CellType.DOUBLE },
                    new PCBI.Automation.IO.Excel.Cell() { Value = IMath.Mils2MM(positionMils.Y), Type = PCBI.Automation.IO.Excel.Cell.CellType.DOUBLE },
                    new PCBI.Automation.IO.Excel.Cell() { Value = cmp.Rotation, Type = PCBI.Automation.IO.Excel.Cell.CellType.DOUBLE },
                    new PCBI.Automation.IO.Excel.Cell() { Value = compType, Type = PCBI.Automation.IO.Excel.Cell.CellType.STRING },
                    new PCBI.Automation.IO.Excel.Cell() { Value = (isPopulated ? "Yes" : "No"), Type = PCBI.Automation.IO.Excel.Cell.CellType.STRING }
                };
                cellList.Add(row);
            }
            PCBISaveFileDlg saveFileDlg = new PCBISaveFileDlg(pcbi);
            saveFileDlg.InitBrowse(initialDir: initialDir, filter: filter, filterIndex: filterIndex, fileName: fileName);
            List<FileFormatFilter> fileFilters = new List<FileFormatFilter>
            {
                new FileFormatFilter("XLSX-Datei (*.xlsx)", ".xlsx"),
                new FileFormatFilter("All files(*.*)", "")
            };
            saveFileDlg.InitDataProviders(loadedDesignPathOrServer: loadedDesignPathOrServer, fileLocation: fileLocation,
                                          fileType: fileType, defaultFileName, fileFilters: fileFilters);
            if (saveFileDlg.ShowDialog() != DialogResult.OK)
                return "The file save dialog has been canceled.";
            IFileData fileData = saveFileDlg.GetFileData();
            if (fileData == null)
                return "No valid file location is selected.";
            if (PCBI.Automation.IO.Excel.WriteExcel(xlsxFile: fileData, SheetName: sheetName, data: cellList, out string error))
                return "The Excel file has been created successfully at: " + fileData.GetFilePath();
            else
                return "Error while creating Excel file: " + error;
        }

        /// <summary>
        /// Example method to generate a component list Excel report by using the PCB-Investigator API
        /// </summary>
        private static string Example_GenerateComponentListExcelReport(IPCBIWindow pcbi, IStep step, CancellationToken? cancelToken, string initialDir, string filter, int filterIndex, string fileName, string loadedDesignPathOrServer, PCBI.Plugin.Interfaces.FileLocation fileLocation, FileType fileType, string sheetName)
        {
            if (!pcbi.JobIsLoaded) return "No job is loaded.";
            List<List<PCBI.Automation.IO.Excel.Cell>> cellList = new List<List<PCBI.Automation.IO.Excel.Cell>>();
            List<PCBI.Automation.IO.Excel.Cell> headerRow = new List<PCBI.Automation.IO.Excel.Cell>
            {
                new PCBI.Automation.IO.Excel.Cell() { Value = "Reference", Type = PCBI.Automation.IO.Excel.Cell.CellType.STRING },
                new PCBI.Automation.IO.Excel.Cell() { Value = "Package name", Type = PCBI.Automation.IO.Excel.Cell.CellType.STRING },
                new PCBI.Automation.IO.Excel.Cell() { Value = "Part number", Type = PCBI.Automation.IO.Excel.Cell.CellType.STRING },
                new PCBI.Automation.IO.Excel.Cell() { Value = "X Position (mm)", Type = PCBI.Automation.IO.Excel.Cell.CellType.STRING },
                new PCBI.Automation.IO.Excel.Cell() { Value = "Y Position (mm)", Type = PCBI.Automation.IO.Excel.Cell.CellType.STRING },
                new PCBI.Automation.IO.Excel.Cell() { Value = "Rotation", Type = PCBI.Automation.IO.Excel.Cell.CellType.STRING },
                new PCBI.Automation.IO.Excel.Cell() { Value = "Mounting Type", Type = PCBI.Automation.IO.Excel.Cell.CellType.STRING },
                new PCBI.Automation.IO.Excel.Cell() { Value = "Populated", Type = PCBI.Automation.IO.Excel.Cell.CellType.STRING }
            };
            cellList.Add(headerRow);
            foreach (ICMPObject cmp in step.GetAllCMPObjects().OrderBy(c => c.Ref))
            {
                if (cancelToken.HasValue && cancelToken.Value.IsCancellationRequested)
                    return "Operation was cancelled.";
        
                // Get the position of the component
                PointD positionMils = cmp.GetPosition(); //always in mils
        
                // Check if the component is populated
                bool isPopulated = IAttribute.GetStandardAttribute(cmp, PCBI.FeatureAttributeEnum.comp_ignore) == null && IAttribute.GetStandardAttribute(cmp, PCBI.FeatureAttributeEnum.no_pop) == null;
                string compType = IAttribute.GetStandardAttribute(cmp, PCBI.FeatureAttributeEnum.comp_mount_type)?.Value?.ToString() ?? "Unknown";
                List<PCBI.Automation.IO.Excel.Cell> row = new List<PCBI.Automation.IO.Excel.Cell>
                {
                    new PCBI.Automation.IO.Excel.Cell() { Value = cmp.Ref, Type = PCBI.Automation.IO.Excel.Cell.CellType.STRING },
                    new PCBI.Automation.IO.Excel.Cell() { Value = cmp.UsedPackageName, Type = PCBI.Automation.IO.Excel.Cell.CellType.STRING },
                    new PCBI.Automation.IO.Excel.Cell() { Value = cmp.PartName, Type = PCBI.Automation.IO.Excel.Cell.CellType.STRING },
                    new PCBI.Automation.IO.Excel.Cell() { Value = IMath.Mils2MM(positionMils.X), Type = PCBI.Automation.IO.Excel.Cell.CellType.DOUBLE },
                    new PCBI.Automation.IO.Excel.Cell() { Value = IMath.Mils2MM(positionMils.Y), Type = PCBI.Automation.IO.Excel.Cell.CellType.DOUBLE },
                    new PCBI.Automation.IO.Excel.Cell() { Value = cmp.Rotation, Type = PCBI.Automation.IO.Excel.Cell.CellType.DOUBLE },
                    new PCBI.Automation.IO.Excel.Cell() { Value = compType, Type = PCBI.Automation.IO.Excel.Cell.CellType.STRING },
                    new PCBI.Automation.IO.Excel.Cell() { Value = (isPopulated ? "Yes" : "No"), Type = PCBI.Automation.IO.Excel.Cell.CellType.STRING }
                };
                cellList.Add(row);
            }
            PCBISaveFileDlg saveFileDlg = new PCBISaveFileDlg(pcbi);
            saveFileDlg.InitBrowse(initialDir: initialDir, filter: filter, filterIndex: filterIndex, fileName: fileName);
            List<FileFormatFilter> fileFilters = new List<FileFormatFilter>
            {
                new FileFormatFilter("XLSX-Datei (*.xlsx)", ".xlsx"),
                new FileFormatFilter("All files(*.*)", "")
            };
            saveFileDlg.InitDataProviders(loadedDesignPathOrServer: loadedDesignPathOrServer, fileLocation: fileLocation,
                                          fileType: fileType, fileName, fileFilters: fileFilters);
            if (saveFileDlg.ShowDialog() != DialogResult.OK)
                return "The file save dialog has been canceled.";
            IFileData fileData = saveFileDlg.GetFileData();
            if (fileData == null)
                return "No valid file location is selected.";
            if (PCBI.Automation.IO.Excel.WriteExcel(xlsxFile: fileData, SheetName: sheetName, data: cellList, out string error))
                return "The Excel file has been created successfully at: " + fileData.GetFilePath();
            else
                return "Error while creating Excel file: " + error;
        }
        
}
}
