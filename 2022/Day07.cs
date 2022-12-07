using AoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2022
{
   //----------------------------------------------------------------------------------------------
   //----------------------------------------------------------------------------------------------
   internal class FileInfo
   {
      public string Name = ""; 
      public Int64 Size = 0; 
   }

   //----------------------------------------------------------------------------------------------
   //----------------------------------------------------------------------------------------------
   internal class FolderInfo
   {
      public FolderInfo? Parent = null; 

      public string Name = ""; 
      public Int64 InclusiveSize = 0;  // size of all files + all sub folders
      public Int64 ExclusiveSize = 0;  // size of all files only

      public List<FileInfo> Files = new(); 
      public List<FolderInfo> Folders = new(); 

      //----------------------------------------------------------------------------------------------
      public void AddFile( string name, Int64 size )
      {
         FileInfo? fi = Files.Find( f => f.Name == name ); 
         if (fi == null) {
            FileInfo newFile = new FileInfo(); 
            newFile.Name = name; 
            newFile.Size = size; 
            Files.Add(newFile); 
         }
      }

      //----------------------------------------------------------------------------------------------
      public void AddFolder( string name )
      {
         FolderInfo? fi = Folders.Find( f => f.Name == name ); 
         if (fi == null) {
            FolderInfo folder = new FolderInfo(); 
            folder.Parent = this; 
            folder.Name = name; 
            Folders.Add(folder); 
         }
      }

      //----------------------------------------------------------------------------------------------
      public FolderInfo? FindFolder( string name )
      {
         return Folders.Find( f => f.Name == name ); 
      }

      //----------------------------------------------------------------------------------------------
      public bool IsRoot()
      {
         return Parent == null;
      }

      //----------------------------------------------------------------------------------------------
      public Int64 ComputeFolderSizes()
      {
         Int64 folderSize = 0;
         foreach (FolderInfo folder in Folders) {
            folderSize += folder.ComputeFolderSizes();
         }

         Int64 fileSize = 0; 
         foreach (FileInfo file in Files) {
            fileSize += file.Size; 
         }

         InclusiveSize = folderSize + fileSize; 
         ExclusiveSize = fileSize; 

         return InclusiveSize; 
      }

      //----------------------------------------------------------------------------------------------
      public List<FolderInfo> FindAllFolders( Func<FolderInfo, bool> pred)
      {
         List<FolderInfo> allFolders = new(); 
         foreach (FolderInfo fi in Folders) {
            List<FolderInfo> subFolders = fi.FindAllFolders(pred); 
            if (pred(fi)) {
               allFolders.Add(fi); 
            }
            allFolders.AddRange(subFolders); 
         }

         return allFolders; 
      }
   }

   //----------------------------------------------------------------------------------------------
   //----------------------------------------------------------------------------------------------
   internal class Day07 : Day
   {
      private string InputFile = "2022/inputs/07.txt";
      
      private List<string> Lines = new(); 
      public FolderInfo Root = new FolderInfo(); 
      public FolderInfo? CWD = null; 

      //----------------------------------------------------------------------------------------------
      void ResetCWD()
      {
         CWD = Root; 
      }

      //----------------------------------------------------------------------------------------------
      void PopCWD()
      {
         if ((CWD == null) || CWD.IsRoot()) {
            CWD = Root; 
         } else {
            CWD = CWD.Parent; 
         }
      }

      //----------------------------------------------------------------------------------------------
      void PushCWD(string folderName)
      {
         if (CWD == null) {
            return; 
         }

         CWD = CWD.FindFolder(folderName); 
      }

      //----------------------------------------------------------------------------------------------
      private void ParseCommand(string[] tokens)
      {
         string command = tokens[1]; 
         if (command == "cd") {
            string where = tokens[2]; 
            if (where == "/") {
               ResetCWD(); 
            } else if (where == "..") {
               PopCWD(); 
            } else {
               PushCWD(where); 
            }
         } else {
            // nothing, don't care about list commands
         }
      }

      //----------------------------------------------------------------------------------------------
      private void AddDirectory(string name)
      {
         CWD!.AddFolder(name); 
      }

      //----------------------------------------------------------------------------------------------
      private void AddFile(string name, Int64 size)
      {
         CWD!.AddFile(name, size); 
      }

      //----------------------------------------------------------------------------------------------
      public override void ParseInput()
      {
         List<string> lines = Util.ReadFileToLines(InputFile);

         foreach (string line in lines) {
            string[] tokens = line.Split(' '); 
            if (tokens[0] == "$") {
               ParseCommand(tokens); 
            } else if (tokens[0] == "dir") {
               AddDirectory(tokens[1]); 
            } else {
               AddFile(tokens[1], Int64.Parse(tokens[0])); 
            }
         }

         Root.ComputeFolderSizes(); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunA()
      {
         List<FolderInfo> folders = Root.FindAllFolders( f => f.InclusiveSize <= 100000 ); 

         Int64 sum = 0; 
         foreach (FolderInfo folder in folders) {
            sum += folder.InclusiveSize; 
         }

         return sum.ToString(); 
      }

      //----------------------------------------------------------------------------------------------
      public override string RunB()
      {
         Int64 spaceNeeded = 30000000; 
         Int64 spaceFree = 70000000 - Root.InclusiveSize;
         Int64 toFree = spaceNeeded - spaceFree; 

         List<FolderInfo> folders = Root.FindAllFolders( f => f.InclusiveSize >= toFree ); 
         FolderInfo? minFolder = folders.MinBy( f => f.InclusiveSize ); 
         return minFolder!.InclusiveSize.ToString(); 
      }
   }
}
