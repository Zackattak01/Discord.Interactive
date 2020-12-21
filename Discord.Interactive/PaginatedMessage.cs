using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Discord;
using Discord.Commands;

namespace Name
{
    public class PaginatedMessage
    {
        public IReadOnlyCollection<Embed> Pages { get; private set; }
        public EmbedAuthorBuilder DefaultAuthor { get; private set; }
        public EmbedFooterBuilder DefaultFooter { get; private set; }
        public Color? DefaultColor { get; private set; }
        public int FieldsPerPage { get; private set; }

        private int currentPage;

        public PaginatedMessage(EmbedAuthorBuilder author = null, EmbedFooterBuilder footer = null,
                                Color? color = null, int? fieldsPerPage = null)
        {
            currentPage = -1;

            DefaultAuthor = author;
            DefaultFooter = footer;
            FieldsPerPage = fieldsPerPage ?? 5;
            DefaultColor = color;

            Pages = new List<Embed>();
        }

        public PaginatedMessage AddPage(EmbedBuilder embed)
        {
            if (embed.Author is null)
                embed.Author = DefaultAuthor;

            if (embed.Footer is null)
                embed.Footer = DefaultFooter;

            if (embed.Color is null)
                embed.Color = DefaultColor;

            var embeds = Pages.ToList();
            embeds.Add(embed.Build());
            Pages = embeds;

            return this;
        }

        public PaginatedMessage AddPage(string embed)
        {
            EmbedBuilder eb = new EmbedBuilder();

            eb.WithDescription(embed);

            AddPage(eb);

            return this;
        }

        public PaginatedMessage AddPages(IEnumerable<EmbedBuilder> embeds)
        {
            foreach (var embed in embeds)
            {
                AddPage(embed);
            }
            return this;
        }

        public PaginatedMessage AddPages(IEnumerable<string> embeds)
        {
            foreach (var embed in embeds)
            {
                AddPage(embed);
            }
            return this;
        }

        public PaginatedMessage AddPagesFromFields(IEnumerable<EmbedFieldBuilder> fields, int? fieldsPerPage = null)
        {

            var fieldList = fields.ToList();

            while (!(fieldList.Count <= 0))
            {
                var fieldsToAdd = fieldList.Take(fieldsPerPage ?? FieldsPerPage).ToList();

                fieldList.RemoveRange(0, fieldsToAdd.Count);

                var eb = new EmbedBuilder();
                eb.Fields = fieldsToAdd;

                AddPage(eb);
            }

            return this;
        }

        internal Embed NextPage()
        {
            currentPage++;

            if (currentPage >= Pages.Count)
                return null;

            return Pages.ElementAt(currentPage);
        }

        internal Embed PreviousPage()
        {
            if (currentPage <= 0)
            {
                currentPage = 0;
                return null;
            }

            currentPage--;

            return Pages.ElementAt(currentPage);
        }

        internal Embed FirstPage()
            => Pages.ElementAt(0);

        internal Embed LastPage()
            => Pages.ElementAt(Pages.Count - 1);

        internal bool IsValidPaginatedMessage()
            => Pages.Count > 0;
    }
}