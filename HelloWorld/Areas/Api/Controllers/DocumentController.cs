using StudyOnline.Areas.Master.Controllers;
using StudyOnline.Models;
using StudyOnline.Results;
using StudyOnline.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace StudyOnline.Areas.Api.Controllers
{
    public class DocumentController : Controller
    {
        StudyOnline.Models.StudyOnlineEntities entities = new StudyOnline.Models.StudyOnlineEntities();

        /// <summary>
        /// 取得指定文件夹下的文档列表记录
        /// </summary>
        /// <param name="folderId">指定的文件夹Id</param>
        /// <param name="userId">用户Id,因为该文件夹有可能涉及权限</param>
        /// <param name="skip">跳过记录数</param>
        /// <param name="take">获取记录数</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetListByFolderId(Int32 folderId, Int32? userId, Int32? skip, Int32? take)
        {
            Folder folder = entities.Folder.Find(folderId);
            if (folder == null)
            {
                return Json(new { code = 202, desc = "指定文件夹不存在" });
            }

            var temp = folder.Document.Where(o => o.AuditCase == AuditCase.审核).OrderBy(o => o.Sort).ThenByDescending(o => o.AuditDate).Skip(skip ?? 0).Take(take ?? Int32.MaxValue).Select(o => new
            {
                o.Id,
                o.Title,
                o.TitleTwo,
                TitleCn = o.Title,
                TitleEn = o.TitleTwo,
                o.TitlePy,
                o.TitleSubCn,
                o.TitleSubEn,
                o.TitleSubPy,
                o.Category,
                o.SoundPath,
                o.Duration,
                o.Length,
                o.LengthString,
                o.Contents,
                o.FolderId,
                o.LevelId,
                AuditDate = (o.AuditDate.HasValue ? o.AuditDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : null),
                Date = (o.AuditDate.HasValue ? o.AuditDate.Value.ToString("yyyy-MM-dd") : null),
                Size = o.Length,
                Time = o.Duration
            });

            if (folder.Member.Any())
            {
                DateTime now = DateTime.Now;
                var vfmu = entities.View_Folder_Member_User.Where(o => o.FolderId == folderId && o.UserId == userId && o.From < now && now < o.To).ToList();
                NimUser nimuser = entities.NimUser.Find(userId);


                if (vfmu.Count > 0 || (nimuser != null && nimuser.Category == 1))
                {
                    return Json(new { code = 200, desc = "查询成功", info = temp.ToList() });
                }
                else
                {
                    return Json(new { code = 201, desc = "没有权限" });
                }
            }
            return Json(new { code = 200, desc = "查询成功", info = temp.ToList() });
        }

        public ActionResult GetListByLevelId(Int32 levelId, Int32? skip, Int32? take)
        {
            var temp = entities.Document.Where(o => o.AuditCase == AuditCase.审核 && o.LevelId == levelId).OrderBy(o => o.Sort).ThenByDescending(o => o.AuditDate).Skip(skip ?? 0).Take(take ?? Int32.MaxValue).ToList();
            return new DateTimeSEJsonResult(new
            {
                code = 200,
                desc = "查询成功",
                info = temp.Select(o => new
                {
                    o.Id,
                    TitleCn = o.Title,
                    TitleEn = o.TitleTwo,
                    o.TitlePy,
                    o.TitleSubCn,
                    o.TitleSubEn,
                    o.TitleSubPy,
                    o.LevelId,
                    o.FolderId,
                    o.Sort,
                    o.Cover,
                    o.Category,
                    o.AuditDate,
                    o.Length,
                    o.LengthString,
                    o.Duration,
                    o.SoundPath
                })
            });

        }

        [HttpPost]
        public ActionResult GetListByFolderIdWithoutCheck(Int32 folderId, Int32? skip, Int32? take)
        {
            var temp = entities.Document.Where(o => o.FolderId == folderId).Where(o => o.AuditCase == AuditCase.审核).OrderBy(o => o.Sort).ThenByDescending(o => o.AuditDate).Skip(skip ?? 0).Take(take ?? Int32.MaxValue).ToList();

            return Json(new
            {
                code = 200,
                desc = "查询成功",
                info = temp.Select(o => new
                {
                    o.Id,
                    o.Title,
                    o.TitleTwo,
                    TitleCn = o.Title,
                    TitleEn = o.TitleTwo,
                    o.LevelId,
                    o.FolderId,
                    o.TitlePy,
                    o.Category,
                    o.TitleSubCn,
                    o.TitleSubEn,
                    o.TitleSubPy,
                    o.Length,
                    isFolder = false,
                    Name = o.Title
                })
            });
        }

        [HttpPost]
        public ActionResult GetLyricListByDocumentId(Int32 documentId)
        {
            Document document = entities.Document.Find(documentId);

            //解析歌词
            Lyric lyric = LyricParser.Parse(document.Lyrics);
            List<Line> lines = new List<Line>();
            Regex regex = new Regex(@"\<[\s\S]*\>");
            foreach (var item in lyric.Lines)
            {
                Line line = new Line();
                line.TimeLabel = item.Key;
                line.Original = item.Value.IndexOf('<') > 0 ? item.Value.Substring(0, item.Value.IndexOf('<')) : item.Value;
                line.Translate = regex.Match(item.Value).Value.Replace("<", "").Replace(">", "");
                lines.Add(line);
            }

            //解析中英文
            return Json(new
            {
                code = 200,
                desc = "查询成功",
                info = lines.Select(o => new
                {
                    Name = o.Original,
                    o.TimeLabel.TotalMilliseconds,
                    o.Translate,
                    o.Original
                })
            });
        }

        [HttpPost]
        public ActionResult GetById(Int32 id)
        {
            Document document = entities.Document.Find(id);
            if (document == null)
            {
                return Json(new { code = 201, desc = "文档不存在" });
            }

            //解析歌词
            Lyric lyric = LyricParser.Parse(document.Lyrics);
            List<Line> lines = new List<Line>();
            Regex regex = new Regex(@"\<[\s\S]*\>");
            foreach (var item in lyric.Lines)
            {
                Line line = new Line();
                line.TimeLabel = item.Key;
                line.Original = item.Value.IndexOf('<') > 0 ? item.Value.Substring(0, item.Value.IndexOf('<')) : item.Value;
                line.Translate = regex.Match(item.Value).Value.Replace("<", "").Replace(">", "");
                lines.Add(line);
            }

            return Json(new
            {
                code = 200,
                desc = "查询成功",
                info = new
                {
                    document.Id,
                    document.Sort,
                    document.Title,
                    document.TitleTwo,
                    TitleCn = document.Title,
                    TitleEn = document.TitleTwo,
                    document.TitlePy,
                    document.Category,
                    document.SoundPath,
                    document.TitleSubCn,
                    document.TitleSubEn,
                    document.TitleSubPy,
                    Lyrics = lines.Select(o => new { o.Original, o.Translate, o.TimeLabel.TotalMilliseconds }),
                    document.FolderId,
                    Folder = (document.FolderId.HasValue ? new
                    {
                        document.Folder.Id,
                        document.Folder.Name,
                        document.Folder.NameEn,
                        document.Folder.Sort
                    } : null)
                }
            });
        }

        /// <summary>
        /// 文档播放统计
        /// </summary>
        /// <param name="id">文档Id</param>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Count(Int32 id, Int32? userId)
        {
            Playlist playlist = new Playlist() { Id = Guid.NewGuid().ToString().Replace("-", ""), DocumentId = id, UserId = userId.HasValue && userId.Value > 0 ? userId : null, PlayAt = DateTime.Now };
            entities.Playlist.Add(playlist);
            entities.SaveChanges();
            return Json(new { code = 200, desc = "统计成功" });
        }
    }
}