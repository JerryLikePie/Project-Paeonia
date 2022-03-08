using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    public class DramaParser : MonoBehaviour
    {
        const int STAT_CHAPTER_START        = 0;
        const int STAT_CHAPTOR_END          = 1;
        const int STAT_SECTION_START        = 1;
        const int STAT_SECTION_BACKGROUND   = 2;
        const int STAT_SECTION_CHARACTORS   = 3;
        const int STAT_SECTION_CONVERSATION = 4;
        const int STAT_SECTION_END          = 4;
        const int STAT_ERROR                = -1;
        const int STAT_FINISH               = 9;
        public static DramaData parseDramaText(string[] lines)
        {
            int stat = STAT_CHAPTER_START;
            DramaData data = new DramaData();
            DramaSectionData curSection = new DramaSectionData();
            DramaCharactorData curCharactor = new DramaCharactorData();
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                string lineUpper = line.ToUpper();
                switch(stat)
                {
                    case STAT_CHAPTER_START:
                        if (line.Length == 0) continue;
                        if (lineUpper.StartsWith("CHAPTOR:") || lineUpper.StartsWith("CHAPTOR："))
                        {
                            line = line.Substring(8).Trim();
                            data.chaptorName = line;
                            stat = STAT_SECTION_START;
                        } else
                        {
                            stat = STAT_ERROR;
                        }
                        break;
                    case STAT_SECTION_START & STAT_CHAPTOR_END:
                        if (line.Length == 0) continue;
                        if (lineUpper.StartsWith("SECTION:") || lineUpper.StartsWith("SECTION："))
                        {
                            line = line.Substring(8).Trim();
                            data.sections.Add(curSection = new DramaSectionData()); 
                            curSection.sectionName = line;
                            stat = STAT_SECTION_BACKGROUND;
                        }
                        else if (lineUpper.Equals("CHAPTOR END"))
                        {
                            stat = STAT_FINISH;
                        }
                        else
                        {
                            stat = STAT_ERROR;
                        }
                        break;
                    case STAT_SECTION_BACKGROUND:
                        if (line.Length == 0) continue;
                        if (lineUpper.StartsWith("BACKGROUND:") || lineUpper.StartsWith("BACKGROUND: "))
                        {
                            line = line.Substring(11).Trim();
                            Sprite img = loadImage(line);
                            if (img != null)
                            {
                                curSection.backgroundImage = img;
                                stat = STAT_SECTION_CHARACTORS;
                            }
                            else
                            {
                                stat = STAT_ERROR;
                            }
                        } else
                        {
                            stat = STAT_ERROR;
                        }
                        break;
                    case STAT_SECTION_CHARACTORS:
                        if (line.Length == 0) continue;
                        if (lineUpper.StartsWith("CHARACTORS:") || lineUpper.StartsWith("CHARACTORS: "))
                        {
                            line = line.Substring(11).Trim();
                            if (!line.Contains(";"))
                            {
                                stat = STAT_ERROR;
                                continue;
                            }
                            string[] chStrs = line.Split(';');
                            foreach (string chStr in chStrs)
                            {
                                if (!chStr.Contains("|"))
                                {
                                    stat = STAT_ERROR;
                                    continue;
                                }
                                string[] chInfo = chStr.Split('|');
                                if (chInfo.Length == 2)
                                {
                                    curCharactor = new DramaCharactorData();
                                    curCharactor.chName = chInfo[0].Trim();
                                    curCharactor.chAbbr = chInfo[0].Trim(); // 缩写与原名相同
                                    Sprite img = loadImage(chInfo[1].Trim());
                                    if (img == null)
                                    {
                                        stat = STAT_ERROR;
                                        continue;
                                    }
                                    curCharactor.chImage = img;
                                    curSection.charactors.Add(curCharactor);
                                } else if (chInfo.Length == 3)
                                {
                                    curCharactor = new DramaCharactorData();
                                    curCharactor.chName = chInfo[0].Trim();
                                    curCharactor.chAbbr = chInfo[1].Trim();
                                    Sprite img = loadImage(chInfo[2].Trim());
                                    if (img == null)
                                    {
                                        stat = STAT_ERROR;
                                        goto SKIP_FLAG_SECTION_CHARACTORS;
                                    }
                                    curCharactor.chImage = img;
                                    curSection.charactors.Add(curCharactor);
                                } else
                                {
                                    stat = STAT_ERROR;
                                    goto SKIP_FLAG_SECTION_CHARACTORS;
                                }
                                stat = STAT_SECTION_CONVERSATION;
                            }
                        }
                    SKIP_FLAG_SECTION_CHARACTORS:
                        break;
                    case (STAT_SECTION_CONVERSATION & STAT_SECTION_END):
                        // 对话
                        if (line.Length == 0) continue;
                        if (lineUpper.Equals("SECTION END"))
                        {
                            stat = (STAT_CHAPTOR_END & STAT_SECTION_START);
                            continue;
                        }
                        if (!line.Contains(":") && !line.Contains("：")) {
                            stat = STAT_ERROR;
                            continue; 
                        }
                        string chName, chAbbr;
                        int index = -1;
                        for (int j = 0; j < curSection.charactors.Count; j++) {
                            DramaCharactorData ch = curSection.charactors[j];
                            chName = ch.chName;
                            chAbbr = ch.chAbbr;
                            if (line.StartsWith(chName + ":") || line.StartsWith(chName + "："))
                            {
                                line = line.Substring(chName.Length + 1);
                                index = j;
                                break;
                            }
                            else if (line.StartsWith(chAbbr + ":") || line.StartsWith(chAbbr + "："))
                            {
                                line = line.Substring(chAbbr.Length + 1);
                                index = j;
                                break;
                            }
                        }
                        if (index >= 0)
                        {
                            curSection.conversations.Add(new DramaConversationData(index, line));
                            stat = STAT_SECTION_END;
                        }
                        else
                        {
                            stat = STAT_ERROR;
                        }
                        break;
                    case STAT_FINISH:
                        return data;
                    case STAT_ERROR:
                        // todo on error
                        return null;
                    default:
                        break;
                }
            }
            if (stat == STAT_FINISH)
            {
                return data;
            }
            return null;
        }

        private static Sprite loadImage(string fileName)
        {
            Object image = Resources.Load("DramaRes/" + fileName, typeof(Sprite));
            Sprite temp;
            try
            {
                temp = Instantiate(image) as Sprite;
            } 
            catch (System.Exception)
            {
                temp = null;
            }
            return temp;
        }
    }
}