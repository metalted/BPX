using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace BPX
{
    public class BPXPanelExplorerDirectory
    {
        public DirectoryInfo directoryInfo;
        public string DisplayName;
    }

    public class BPXPanelExplorerFile
    {
        public FileInfo fileInfo;
        public string DisplayName;
    }

    public class BPXPanelExplorerContents
    {
        public List<BPXPanelExplorerDirectory> directories;
        public List<BPXPanelExplorerFile> files;

        public BPXPanelExplorerContents(DirectoryInfo directory, string search)
        {
            directories = new List<BPXPanelExplorerDirectory>();
            files = new List<BPXPanelExplorerFile>();

            if (search != "")
            {
                FileInfo[] f = directory.GetFiles("*.zeeplevel", SearchOption.AllDirectories);
                f = f.Where(file => file.Name.ToUpper().Contains(search.ToUpper())).ToArray();

                foreach (FileInfo fi in f)
                {
                    BPXPanelExplorerFile file = new BPXPanelExplorerFile();
                    file.fileInfo = fi;
                    file.DisplayName = fi.Name.Replace(".zeeplevel", "");
                    files.Add(file);
                }
            }
            else
            {
                FileInfo[] f = directory.GetFiles("*.zeeplevel");
                Debug.Log(f.Length);
                DirectoryInfo[] d = directory.GetDirectories();
                Debug.Log(d.Length);
                foreach (DirectoryInfo di in d)
                {
                    Debug.Log(di.FullName);
                    bool addThisDirectory = true;

                    FileInfo[] diFiles = di.GetFiles();

                    bool hasZeeplevels = false;
                   
                    string firstZeeplevelName = "";

                    if (diFiles.Length > 0)
                    {
                        foreach (FileInfo fi in diFiles)
                        {
                            string ext = fi.Extension.ToLower();
                            Debug.Log(ext);
                            if (ext == ".zeeplevel")
                            {
                                if (!hasZeeplevels)
                                {
                                    firstZeeplevelName = Path.GetFileNameWithoutExtension(fi.FullName);
                                    hasZeeplevels = true;
                                }
                            }
                            else
                            {
                                if (!BPXConfiguration.IsAllowedExtension(ext))
                                {
                                    addThisDirectory = false;
                                }
                            }
                        }
                    }

                    Debug.Log(hasZeeplevels);

                    if (addThisDirectory)
                    {
                        BPXPanelExplorerDirectory dir = new BPXPanelExplorerDirectory();
                        dir.directoryInfo = di;
                        dir.DisplayName = di.Name;

                        //Check if its a mod.io folder.
                        if (Regex.IsMatch(Path.GetFileName(di.FullName), @"^\d{7}_\d{7}$"))
                        {
                            if(hasZeeplevels)
                            {
                                dir.DisplayName = "*" + firstZeeplevelName;
                            }
                            else
                            {
                                //If it has a single directory inside, go directly to that directory.
                                DirectoryInfo[] nested = di.GetDirectories();
                                if(nested.Length == 1)
                                {
                                    dir.directoryInfo = nested[0];
                                    dir.DisplayName = nested[0].Name;
                                }
                            }
                        }

                        directories.Add(dir);
                    }
                }

                foreach (FileInfo fi in f)
                {
                    BPXPanelExplorerFile file = new BPXPanelExplorerFile();
                    Debug.Log(fi.FullName);
                    file.fileInfo = fi;
                    file.DisplayName = fi.Name.Replace(".zeeplevel", "");
                    files.Add(file);
                }
            }
        }
    }
}