#!/bin/bash

#Define files, folders, and relative paths
FILE_SRC="BattleEngine.cs"
FOLDER_SRC="cls_files"
BACKUP_BASE="../../bk"
SOURCE_DIR=$(pwd);

#Ensure the base backup folder exists
mkdir -p "$BACKUP_BASE"

#Calculate the next backup folder number
i=$(ls -d "$BACKUP_BASE"/BattleBK_* 2>/dev/null | wc -l)
count=$((i + 1))
newBackupFolder="BattleBK_${count}"
DEST_DIR="$BACKUP_BASE/$newBackupFolder"

echo "Starting Backup Process..."
echo "Making Backup to $DEST_DIR"

#Create the specific backup directory
mkdir -p "$DEST_DIR"

#Copy the single file
echo "|---"
cp "$SOURCE_DIR/$FILE_SRC" "$DEST_DIR/"

#Copy the folder
echo "|-------"
cp -r "$SOURCE_DIR/$FOLDER_SRC" "$DEST_DIR/"

echo "|------------|"
echo "Successfully Made full local backup!"
