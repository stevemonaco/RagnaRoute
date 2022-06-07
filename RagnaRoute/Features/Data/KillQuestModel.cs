using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RagnaRoute.Features.Data;

public record KillQuestModel(string Name, string Description, List<string> Information);
