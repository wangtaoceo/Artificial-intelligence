﻿using System.Collections.Generic;
using System;
using pair = System.Collections.Generic.KeyValuePair<int, int>;

namespace ChineseChess
{
	partial class Controller
	{
		//象棋一共14种
		//黑棋
		//0-車 1-马 2-象 3-士 4-将 5-炮 6-卒
		//红棋
		//7-車 8-马 9-象 10-士 11-帅 12-炮 13-卒
		//車和炮使用一种单独的着法生成器(预置+位棋盘)，其他棋子使用预置着法生成器
		//象棋棋盘大小9*10
		//0   1   2   3   4   5   6   7   8
		//9   10  11  12  13  14  15  16  17
		//18  19  20  21  22  23  24  25  26
		//27  28  29  30  31  32  33  34  35
		//36  37  38  39  40  41  42  43  44
		//45  46  47  48  49  50  51  52  53
		//54  55  56  57  58  59  60  61  62
		//63  64  65  66  67  68  69  70  71
		//72  73  74  75  76  77  78  79  80
		//81  82  83  84  85  86  87  88  89

		//转化为上面注释的编号格式
		/*private int GetChessNum(Player player, ChessType type)
		{
			int chessNum = (int)player * 7;
			switch (type)
			{
				case ChessType.Rook1:
				case ChessType.Rook2:
					break;
				case ChessType.Knight1:
				case ChessType.Knight2:
					chessNum += 1;
					break;
				case ChessType.Elephant1:
				case ChessType.Elephant2:
					chessNum += 2;
					break;
				case ChessType.Mandarin1:
				case ChessType.Mandarin2:
					chessNum += 3;
					break;
				case ChessType.King:
					chessNum += 4;
					break;
				case ChessType.Cannon1:
				case ChessType.Cannon2:
					chessNum += 5;
					break;
				case ChessType.Pawn1:
				case ChessType.Pawn2:
				case ChessType.Pawn3:
				case ChessType.Pawn4:
				case ChessType.Pawn5:
					chessNum += 6;
					break;
				default:
					return -1;
			}
			return chessNum;
		}*/

		//人工智障
		private class AI
		{
			//预置着法生成器
			//存储下一步的走法以及马脚和象眼(其他棋子置0，为多余的存储空间)
			public List<pair>[,] chessSet;
			//车炮预置着法生成器
			public int[,] RookRow;
			public int[,] RookCol;
			public int[,] CannonRow;
			public int[,] CannonCol;

			//价值评估
			//为了减少每次计算黑棋反转的代价存储的时候就存一个翻转数组up side down
			private int[][] value = {
				//卒 & 帅
				new int[90]{ 9,  9,  9, 11, 13, 11,  9,  9,  9,
							19, 24, 34, 42, 44, 42, 34, 24, 19,
							19, 24, 32, 37, 37, 37, 32, 24, 19,
							19, 23, 27, 29, 30, 29, 27, 23, 19,
							14, 18, 20, 27, 29, 27, 20, 18, 14,
							 7,  0, 13,  0, 16,  0, 13,  0,  7,
							 7,  0,  7,  0, 15,  0,  7,  0,  7,
							 0,  0,  0,  1,  1,  1,  0,  0,  0,
							 0,  0,  0,  2,  2,  2,  0,  0,  0,
							 0,  0,  0, 11, 15, 11,  0,  0,  0},

				new int[90]{ 0,  0,  0, 11, 15, 11,  0,  0,  0,
							 0,  0,  0,  2,  2,  2,  0,  0,  0,
							 0,  0,  0,  1,  1,  1,  0,  0,  0,
							 7,  0,  7,  0, 15,  0,  7,  0,  7,
							 7,  0, 13,  0, 16,  0, 13,  0,  7,
							14, 18, 20, 27, 29, 27, 20, 18, 14,
							19, 23, 27, 29, 30, 29, 27, 23, 19,
							19, 24, 32, 37, 37, 37, 32, 24, 19,
							19, 24, 34, 42, 44, 42, 34, 24, 19,
							 9,  9,  9, 11, 13, 11,  9,  9,  9},
				//象 & 士
				new int[90]{ 0,  0, 20, 20,  0, 20, 20,  0,  0,
							 0,  0,  0,  0, 23,  0,  0,  0,  0,
							18,  0,  0, 20, 23, 20,  0,  0, 18,
							 0,  0,  0,  0,  0,  0,  0,  0,  0,
							 0,  0, 20,  0,  0,  0, 20,  0,  0,
							 0,  0, 20,  0,  0,  0, 20,  0,  0,
							 0,  0,  0,  0,  0,  0,  0,  0,  0,
							18,  0,  0, 20, 23, 20,  0,  0, 18,
							 0,  0,  0,  0, 23,  0,  0,  0,  0,
							 0,  0, 20, 20,  0, 20, 20,  0,  0},
				//马
				new int[90]{90, 90, 90, 96, 90, 96, 90, 90, 90,
							90, 96,103, 97, 94, 97,103, 96, 90,
							92, 98, 99,103, 99,103, 99, 98, 92,
							93,108,100,107,100,107,100,108, 93,
							90,100, 99,103,104,103, 99,100, 90,
							90, 98,101,102,103,102,101, 98, 90,
							92, 94, 98, 95, 98, 95, 98, 94, 92,
							93, 92, 94, 95, 92, 95, 94, 92, 93,
							85, 90, 92, 93, 78, 93, 92, 90, 85,
							88, 85, 90, 88, 90, 88, 90, 85, 88},

				new int[90]{88, 85, 90, 88, 90, 88, 90, 85, 88,
							85, 90, 92, 93, 78, 93, 92, 90, 85,
							93, 92, 94, 95, 92, 95, 94, 92, 93,
							92, 94, 98, 95, 98, 95, 98, 94, 92,
							90, 98,101,102,103,102,101, 98, 90,
							90,100, 99,103,104,103, 99,100, 90,
							93,108,100,107,100,107,100,108, 93,
							92, 98, 99,103, 99,103, 99, 98, 92,
							90, 96,103, 97, 94, 97,103, 96, 90,
							90, 90, 90, 96, 90, 96, 90, 90, 90},
				//車
				new int[90]{206,208,207,213,214,213,207,208,206,
							206,212,209,216,233,216,209,212,206,
							206,208,207,214,216,214,207,208,206,
							206,213,213,216,216,216,213,213,206,
							208,211,211,214,215,214,211,211,208,
							208,212,212,214,215,214,212,212,208,
							204,209,204,212,214,212,204,209,204,
							198,208,204,212,212,212,204,208,198,
							200,208,206,212,200,212,206,208,200,
							194,206,204,212,200,212,204,206,194},

				new int[90]{194,206,204,212,200,212,204,206,194,
							200,208,206,212,200,212,206,208,200,
							198,208,204,212,212,212,204,208,198,
							204,209,204,212,214,212,204,209,204,
							208,212,212,214,215,214,212,212,208,
							208,211,211,214,215,214,211,211,208,
							206,213,213,216,216,216,213,213,206,
							206,208,207,214,216,214,207,208,206,
							206,212,209,216,233,216,209,212,206,
							206,208,207,213,214,213,207,208,206},
				//炮
				new int[90]{100,100, 96, 91, 90, 91, 96,100,100,
							 98, 98, 96, 92, 89, 92, 96, 98, 98, 
							 97, 97, 96, 91, 92, 91, 96, 97, 97,
							 96, 99, 99, 98,100, 98, 99, 99, 96,
							 96, 96, 96, 96,100, 96, 96, 96, 96,
							 95, 96, 99, 96,100, 96, 99, 96, 95,
							 96, 96, 96, 96, 96, 96, 96, 96, 96,
							 97, 96,100, 99,101, 99,100, 96, 97,
							 96, 97, 98, 98, 98, 98, 98, 97, 96,
							 96, 96, 97, 99, 99, 99, 97, 96, 96},

				new int[90]{ 96, 96, 97, 99, 99, 99, 97, 96, 96,
							 96, 97, 98, 98, 98, 98, 98, 97, 96,
							 97, 96,100, 99,101, 99,100, 96, 97,
							 96, 96, 96, 96, 96, 96, 96, 96, 96,
							 95, 96, 99, 96,100, 96, 99, 96, 95,
							 96, 96, 96, 96,100, 96, 96, 96, 96,
							 96, 99, 99, 98,100, 98, 99, 99, 96,
							 97, 97, 96, 91, 92, 91, 96, 97, 97,
							 98, 98, 96, 92, 89, 92, 96, 98, 98,
							100,100, 96, 91, 90, 91, 96,100,100},
			};

			public AI()
			{
				int x, y;
				//生成预置着法生成器
				//预置着法生成器的构造
				//卒和将单独放一个棋盘，但是要分红棋和黑棋来放
				//士和象放一个棋盘
				//马的走法单独放一个棋盘
				//总共需要4个棋盘
				chessSet = new List<pair>[90, 4];
				//象、士和马的走法
				int[][] dx = {
					//象
					new int[4]{ 2, -2, -2, 2},
					//士
					new int[4]{ 1, -1, -1, 1},
					//马
					new int[8] { 2, 1, 1, 2, -2, -1, -1, -2 }
				};
				int[][] dy = {
					//象
					new int[4]{ 2, 2, -2, -2},
					//士
					new int[4]{ 1, 1, -1, -1},
					//马
					new int[8] { 1, -2, 2, -1, 1, 2, -2, -1 }
				};
				for(int i = 0; i < 90; ++i)
				{
					x = i % 9;
					y = i / 9;
					//卒 & 将
					chessSet[i, 0] = new List<pair>();
					//兵
					if(y < 7)
					{
						if(y < 5)
						{
							if (y > 0)
								chessSet[i, 0].Add(new pair(i - 9, 0));
							if (x > 0)
								chessSet[i, 0].Add(new pair(i - 1, 0));
							if (x < 8)
								chessSet[i, 0].Add(new pair(i + 1, 0));
						}
						else
						{
							if ((x & 1) == 0)
								chessSet[i, 0].Add(new pair(i - 9, 0));
						}
					}
					//将
					else
					{
						if (x >= 3 && x <= 5)
						{
							if (x > 3)
								chessSet[i, 0].Add(new pair(i - 1, 0));
							if (x < 5)
								chessSet[i, 0].Add(new pair(i + 1, 0));
							if (y > 7)
								chessSet[i, 0].Add(new pair(i - 9, 0));
							if (y < 9)
								chessSet[i, 0].Add(new pair(i + 9, 0));
						}
					}
					chessSet[i, 1] = new List<pair>();
					//卒
					if(y > 2)
					{
						if(y > 4)
						{
							if (y < 9)
								chessSet[i, 1].Add(new pair(i + 9, 0));
							if (x > 0)
								chessSet[i, 1].Add(new pair(i - 1, 0));
							if (x < 8)
								chessSet[i, 1].Add(new pair(i + 1, 0));
						}
						else
						{
							if ((x & 1) == 0)
								chessSet[i, 1].Add(new pair(i + 9, 0));
						}
					}
					//帅
					else
					{
						if (x >= 3 && x <= 5)
						{
							if (x > 3)
								chessSet[i, 1].Add(new pair(i - 1, 0));
							if (x < 5)
								chessSet[i, 1].Add(new pair(i + 1, 0));
							if (y > 0)
								chessSet[i, 1].Add(new pair(i - 9, 0));
							if (y < 2)
								chessSet[i, 1].Add(new pair(i + 9, 0));
						}
					}
					//象 & 士
					chessSet[i, 2] = new List<pair>();
					//象
					if(((x == 2 || x == 6) && (y == 0 || y == 4 || y == 5 || y == 9)) 
						|| ((y == 2 || y == 7) && (x == 0 || x == 4 || x == 8)))
					{
						for(int j = 0; j < 4; ++j)
						{
							int xx = x + dx[0][j];
							int yy = y + dy[0][j];
							if (xx >= 0 && xx <= 8 && yy >= 0 && yy <= 9 && ((y <= 4 && yy <= 4) || (y >= 5 && yy >= 5)))
							{
								//象眼
								int px = x + dx[0][j] / 2;
								int py = y + dy[0][j] / 2;
								chessSet[i, 2].Add(new pair(xx + yy * 9, px + py * 9));
							}
						}
					}
					//士
					if((x == 3 || x == 5) && (y == 0 || y == 2 || y == 7 || y == 9)
						|| (x == 4 && (y == 1 || y == 8)))
					{
						for(int j = 0; j < 4; ++j)
						{
							int xx = x + dx[1][j];
							int yy = y + dy[1][j];
							if (xx >= 3 && xx <= 5 && ((yy >= 0 && yy <= 2) || (yy >= 7 && yy <= 9)))
								chessSet[i, 2].Add(new pair(xx + yy * 9, 0));
						}
					}
					//马
					chessSet[i, 3] = new List<pair>();
					for (int j = 0; j < 8; ++j)
					{
						int xx = x + dx[2][j];
						int yy = y + dy[2][j];
						if(xx >= 0 && xx <= 8 && yy >= 0 && yy <= 9)
						{
							//马脚
							int px = Math.Abs(dx[2][j]) == 2 ? x + dx[2][j] / 2 : x;
							int py = Math.Abs(dy[2][j]) == 2 ? y + dy[2][j] / 2 : y;
							chessSet[i, 3].Add(new pair(xx + yy * 9, px + py * 9));
						}
					}
				}
			}
		}
	}
}
