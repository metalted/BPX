using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPX
{
    public class BPXUndoRedoRegistration
    {
        public List<string> before;
        public List<string> beforeSelection;
        public List<string> after;
        public List<string> afterSelection;
        public List<BlockProperties> blockList;

        public BPXUndoRedoRegistration()
        {
            before = new List<string>();
            beforeSelection = new List<string>();
            after = new List<string>();
            afterSelection = new List<string>();
            blockList = new List<BlockProperties>();
        }

        //This is when we create something
        public void SetBefore(int blockCount)
        {
            before = Enumerable.Repeat((string)null, blockCount).ToList();
        }

        //This is when we do an operation
        public void SetBefore(List<BlockProperties> blockList)
        {
            this.blockList = blockList;
            before = BPXManager.central.undoRedo.ConvertBlockListToJSONList(blockList);
            beforeSelection = BPXManager.central.undoRedo.ConvertSelectionToStringList(blockList);
        }

        public void GenerateAfter()
        {
            after = BPXManager.central.undoRedo.ConvertBlockListToJSONList(blockList);
            afterSelection = BPXManager.central.undoRedo.ConvertSelectionToStringList(blockList);
        }

        public Change_Collection CreateCollection()
        {
            //Convert all the before and after data into a Change_Collection.
            Change_Collection collection = BPXManager.central.undoRedo.ConvertBeforeAndAfterListToCollection(
                before, after,
                blockList,
                beforeSelection, afterSelection);

            return collection;
        }
    }
}
