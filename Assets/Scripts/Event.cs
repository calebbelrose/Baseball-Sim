using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Event
{
	public abstract void Action(AllTeams allTeams);
}

public class WorldBaseballClassic : Event
{
	public override void Action(AllTeams allTeams)
	{
		List<Player> players = new List<Player>(), result;
		int maxIndex = 15;

		for(int i = 0; i < allTeams.teams.Count; i++)
			for(int j = 0; j < allTeams.teams[i].players.Count; j++)
				players.Add(allTeams.teams[i].players[j]);

		result = players.OrderBy (playerX => playerX.country).ThenByDescending(playerX => playerX.overall).ToList ();


	}
}

public class OpeningDay : Event
{
	public override void Action(AllTeams allTeams)
	{

	}
}

public class FuturesGame : Event
{
	public override void Action(AllTeams allTeams)
	{

	}
}

public class HomerunDerby : Event
{
	public override void Action(AllTeams allTeams)
	{
		List<Player> players = new List<Player>(), result;
		int maxIndex = 7;

		for(int i = 0; i < allTeams.teams.Count; i++)
			for(int j = 0; j < allTeams.teams[i].players.Count; j++)
				players.Add(allTeams.teams[i].players[j]);

		result = players.OrderByDescending (playerX => playerX.homeruns).ToList ();

		while (maxIndex != 0)
		{
			List<int> losers = new List<int> ();

			for (int index1 = 0, index2 = maxIndex; index1 < index2; index1++)
			{
				int[] scores = new int[2];
				int[] indexes = new int[2];

				indexes [0] = index1;
				indexes [1] = index2--;

				for (int player = 0; player < 2; player++)
				{
					float time = 240.0f;
					bool needExtraTime = true;
					int extraTimeHomeruns = 0;

					while (time > 0.0f)
					{
						float value = (Random.value * result [indexes [player]].skills [0] + Random.value * result [indexes [player]].skills [1] + Random.value * result [indexes [player]].skills [2]);
						float distance = value * 2.5f;

						if (value > 200.0f)
							scores [player]++;

						if (distance >= 440.0f && needExtraTime && ++extraTimeHomeruns == 2)
						{
							time += 30.0f;
							needExtraTime = false;
						}
						Debug.Log (distance + " " + value + " " + scores[0] + " " + scores[1]);
						time -= distance / 50;
					}
				}

				if (scores [0] > scores [1])
					losers.Add (indexes [1]);
				else if (scores [1] > scores [0])
					losers.Add (indexes [0]);
				else
				{
					int swings = 0;

					for (int player = 0; player < 2; player++)
					{
						float time = 60.0f;

						while (time > 0.0f)
						{
							float value = (Random.value * result [indexes [player]].skills [0] + Random.value * result [indexes [player]].skills [1] + Random.value * result [indexes [player]].skills [2]);

							if (value > 200.0f)
								scores [player]++;

							time -= value * 0.05f;
						}
					}

					while (scores [0] == scores [1] && swings >= 3)
					{
						for (int player = 0; player < 2; player++)
						{
							float value = (Random.value * result [indexes [player]].skills [0] + Random.value * result [indexes [player]].skills [1] + Random.value * result [indexes [player]].skills [2]);

							if (value > 200.0f)
								scores [player]++;
							
							swings++;
						}
					}

					if (scores [0] > scores [1])
						losers.Add (indexes [1]);
					else
						losers.Add (indexes [0]);
				}
				Debug.Log (scores [0] + " " + scores [1]);
			}

			losers = losers.OrderByDescending (loser => loser).ToList ();


			while (losers.Count > 0)
			{
				players.RemoveAt (losers [0]);
				losers.RemoveAt (0);
			}

			maxIndex /= 2;
		}
	}
}

public class AllStarGame : Event
{
	public override void Action(AllTeams allTeams)
	{

	}
}

public class SigningDeadline : Event
{
	public override void Action(AllTeams allTeams)
	{

	}
}

public class HallOfFameInduction : Event
{
	public override void Action(AllTeams allTeams)
	{
		double bestScore = 0.0;
		int bestIndex = 0;

		for(int i = 0; i < allTeams.hallOfFameCandidates.Count; i++)
		{
			double score;

			if(allTeams.hallOfFameCandidates[i].position.Contains("P"))
			{
				double era = allTeams.hallOfFameCandidates[i].earnedRuns * 27 / (double)allTeams.hallOfFameCandidates[i].inningsPitched;
				score = (6.0 - era) * 5 + allTeams.hallOfFameCandidates[i].inningsPitched / (double)8;
			}
			else
			{
				double ops = (allTeams.hallOfFameCandidates[i].hits + allTeams.hallOfFameCandidates[i].walks) / (double)(allTeams.hallOfFameCandidates[i].atBats + allTeams.hallOfFameCandidates[i].walks + allTeams.hallOfFameCandidates[i].sacrifices) + (allTeams.hallOfFameCandidates[i].hits + allTeams.hallOfFameCandidates[i].doubles * 2 + allTeams.hallOfFameCandidates[i].triples * 3 + allTeams.hallOfFameCandidates[i].homeruns * 4) / (double)allTeams.hallOfFameCandidates[i].atBats;
				score = allTeams.hallOfFameCandidates[i].homeruns / 40.0 + ops * 25;
			}

			if (score > bestScore)
			{
				bestScore = score;
				bestIndex = i;
			}
		}

		allTeams.hallOfFameInductees.Add (new HallOfFameInductee (allTeams.Year, allTeams.hallOfFameCandidates[bestIndex]));
		allTeams.hallOfFameCandidates.RemoveAt (bestIndex);
	}
}

public class NonWaiverTradeDeadline : Event
{
	public override void Action(AllTeams allTeams)
	{
		allTeams.tradeDeadline = TradeDeadline.NonWaiver;
	}
}

public class WaiverTradeDeadline : Event
{
	public override void Action(AllTeams allTeams)
	{
		allTeams.tradeDeadline = TradeDeadline.Waiver;
	}
}

public class ActiveRosterExpansion : Event
{
	public override void Action(AllTeams allTeams)
	{
		allTeams.ChangeRosterSize (40);
	}
}

public class EndOfRegularSeason : Event
{
	public override void Action(AllTeams allTeams)
	{
		allTeams.ChangeRosterSize (25);
		GameObject.Find("SceneManager").GetComponent<ChangeScene>().ChangeToScene(6);
	}
}

public enum TradeDeadline
{
	None,
	Waiver,
	NonWaiver
}