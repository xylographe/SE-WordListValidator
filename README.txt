
    SE-WordListValidator is a very simple program to validate the XML and
    regular expression syntax of the lists that come with Subtitle Edit,
    located in "%APPDATA%\Subtitle Edit\Dictionaries".

    Mainly useful when you want to submit a pull request to merge your
    local improvements in the official distribution.

    1. Select the modified list in the tree view on the left.

    2. Select "Validate" in the context menu (or doubble click the selected
       item).

    3. If errors were found, select "Edit" in the context menu (or double
       click anywhere in the report view on the right).
       [Starts your system's default XML editor, unless you have set the
        XML_EDITOR environment variable to a different application.]
       Repeat steps 2 and 3 until all syntax errors have been fixed.

    4. Select "Submit" in the context menu to open the corresponding page
       in the SubtitleEdit GitHub repository.

    5. Use "Select All" (Ctrl-A) and "Paste" (Ctrl-V) in your browser to
       replace the list file content with your modified version.

    6. Click "Preview changes".  If all is well, create a pull request,
       then go back to SE-WordListValidator and select "Accept" in the
       context menu to update your local list file.

    SE-WordListValidator does not change the original list file until you
    choose "Accept".  Modifications are kept in a working copy.  Even if
    SE-WordListValidator or Windows should crash, no harm will befall your
    data.  On the next start SE-WordListValidator will ask you if it should
    re-use the previous working copy or create a new one.

    If at any time you want to re-start with the original list file, select
    "Reject" in the context menu to remove the working copy for that list.
