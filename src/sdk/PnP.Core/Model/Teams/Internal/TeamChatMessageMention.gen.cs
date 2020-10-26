﻿namespace PnP.Core.Model.Teams
{
    [GraphType]
    internal partial class TeamChatMessageMention : BaseDataModel<ITeamChatMessageMention>, ITeamChatMessageMention
    {
        public int Id { get => GetValue<int>(); set => SetValue(value); }

        public string MentionText { get => GetValue<string>(); set => SetValue(value); }

        public ITeamIdentitySet Mentioned { get => GetModelValue<ITeamIdentitySet>(); }

        [KeyProperty(nameof(Id))]
        public override object Key { get => this.Id; set => this.Id = int.Parse(value.ToString()); }
    }
}
