using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeakUp
{
	using static Pronoums;
	public class Verb_ToBe : Verbs
    {
		protected new Dictionary<pronoum, string> conjugation = new Dictionary<pronoum, string>()
		{
			{ pronoum.I, "I am" },
			{ pronoum.You, "you are" },
			{ pronoum.He, "he is" },
			{ pronoum.She, "she is" },
			{ pronoum.It, "it is" },
			{ pronoum.We, "we are" },
			{ pronoum.They, "they are" }
		};

	}
}
