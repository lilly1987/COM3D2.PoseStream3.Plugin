using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityInjector;
using UnityInjector.Attributes;

namespace COM3D2.PoseStream.Plugin
{
	#region PluginMain
	///=========================================================================
	/// <summary>動くモーション作成プラグイン</summary>
	/// <remarks>
	///	COM3D2.PoseStream.Plugin : 複数のポーズをつなぎ合わせて一つのanmファイルを生成する UnityInjector/Sybaris 用クラス
	///
	/// </remarks>
	///=========================================================================
	[PluginFilter( "COM3D2x64" ), PluginName("COM3D2.PoseStream.Plugin"), PluginVersion( "1.1.0.0 edit by lilly 001" )]
	public class PoseStream : PluginBase
	{
        public const string Label = "PoseStream 1.1.0.0 edit by lilly 001";
        
        //PoseStream용 변수
		private bool isStudio = false;
		private bool isGUI = false;
		private String anmName = "";
		private String resultMessage = "";

		public void Awake()
		{
			try
			{
				GameObject.DontDestroyOnLoad(this);
				SceneManager.sceneLoaded += OnSceneLoaded;
			}
			catch (Exception e)
			{
				Debug.LogError(e.ToString());
			}
		}

        //화면 업데이트시 호출됨
		public void Update()
		{
			try
			{
				//if (isStudio)
				//{
                    //shift+m
					if (
                        (Input.GetKey(KeyCode.LeftShift) 
                        || Input.GetKey(KeyCode.RightShift)
                        ) &&
						Input.GetKeyDown(KeyCode.M))                    
                    //shift+ctr+m
					//if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) &&
					//	(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) &&
					//	Input.GetKeyDown(KeyCode.M))
					{
						isGUI = !isGUI;
						resultMessage = "";
					}
				//}
			}
			catch (Exception e)
			{
				Debug.LogError(e.ToString());
			}
		}
        
        
		private String getPoseDataPath(bool b )
		{
			String path = Application.dataPath;
			path = path.Substring(0, path.Length - @"COM3D2x64_Data".Length);
            
			path += @"PhotoModeData\MyPose";
			if (b)
			{
				path += @"\";
			}
			return path;
		}

        //장면이 바뀔때마다
		private void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
		{
            //Debug.Log("PoseStream scene.name: "+scene.name);
            //Debug.Log("PoseStream scene.buildIndex: "+scene.buildIndex);
			//try
			//{
            //    //장면 번호(buildIndex)가 27일때 (1.07 기준)
            //    //장면 번호(buildIndex)가 26일때 (1.08 기준)
			//	if (scene.name == "ScenePhotoMode")
			//	//if (scene.buildIndex == 26)
			//	{
			//		//スタジオモード
			//		isStudio = true;
			//	}
			//	else
			//	{
			//		//スタジオモード以外
			//		isStudio = false;
			//	}
			//}
			//catch (Exception e)
			//{
			//	Debug.LogError(e.ToString());
			//}
		}

		private enum GUIPARAM
		{
			S, W, H, X, Y, EXEBW, EXEBH, EXEBX, EXEBY, TW, TH, TX, TY, LW, LH, LX, LY, RW, RH, RX, RY, XW, XH, XX, XY
		}

        
		private int getGUIparam(GUIPARAM p)
		{
			switch (p)
			{
				case GUIPARAM.S:
					return 16;
				case GUIPARAM.W:
					return Screen.width / 3;
                    //메인 프레임 크기
				case GUIPARAM.H:
					return Screen.height / 3;
				case GUIPARAM.X:
					return (Screen.width - getGUIparam(GUIPARAM.W)) / 2;
				case GUIPARAM.Y:
					return (Screen.height - getGUIparam(GUIPARAM.H)) / 2;
				case GUIPARAM.EXEBW:
					return getGUIparam(GUIPARAM.W) / 5;
				case GUIPARAM.EXEBH:
					return getGUIparam(GUIPARAM.H) / 5;
				case GUIPARAM.EXEBX:
					return getGUIparam(GUIPARAM.W) / 40;
				case GUIPARAM.EXEBY:
					return getGUIparam(GUIPARAM.H) * 6 / 10;
				case GUIPARAM.TW:
					return getGUIparam(GUIPARAM.W) * 4 / 5;
				case GUIPARAM.TH:
					return getGUIparam(GUIPARAM.H) / 5;
				case GUIPARAM.TX:
					return getGUIparam(GUIPARAM.W) / 40;
				case GUIPARAM.TY:
					return getGUIparam(GUIPARAM.H) * 1 / 10;
				case GUIPARAM.LW:
					return getGUIparam(GUIPARAM.W) * 4 / 5;
				case GUIPARAM.LH:
					return getGUIparam(GUIPARAM.H) / 5;
				case GUIPARAM.LX:
					return getGUIparam(GUIPARAM.TX);
				case GUIPARAM.LY:
					return getGUIparam(GUIPARAM.TY) + getGUIparam(GUIPARAM.TH);
				case GUIPARAM.RW:
					return getGUIparam(GUIPARAM.W) - getGUIparam(GUIPARAM.EXEBX) + getGUIparam(GUIPARAM.EXEBW);
				case GUIPARAM.RH:
					return getGUIparam(GUIPARAM.H) * 2 / 5;
				case GUIPARAM.RX:
					return getGUIparam(GUIPARAM.EXEBX) + getGUIparam(GUIPARAM.EXEBW);
				case GUIPARAM.RY:
					return getGUIparam(GUIPARAM.EXEBY);
				case GUIPARAM.XW:
					return getGUIparam(GUIPARAM.W) / 15;
				case GUIPARAM.XH:
					return getGUIparam(GUIPARAM.W) / 15;
				case GUIPARAM.XX:
					return getGUIparam(GUIPARAM.W) * 14 / 15;
				case GUIPARAM.XY:
					return 0;
				default:
					return 0;
			}
		}

        
		private String anmMake(bool minAnmMake)
		{
			if (anmName.Length == 0)
			{
				return myError("名前が入力されていません.\n이름이 입력되어 있지 않습니다");
			}
			String[] files = Directory.GetFiles(getPoseDataPath(false), anmName + @"_????????.anm", SearchOption.TopDirectoryOnly);
			if (files == null || files.Length == 0)
			{
				return myError("連結対象のポーズ연결 대상의 포즈\n(" + anmName + "_00000000～)\nがありません이 없습니다");
			}
			List<String> lf = new List<string>(files);
			List<String> ps = new List<string>();
            Debug.Log("PoseStream3 ps.Count : " + ps.Count);
           
            foreach (String s in lf)
			{
				String s2 = s.Substring(getPoseDataPath(true).Length);
				if (s2.Length != (anmName.Length + @"_00000000.anm".Length))
				{
					//8桁でない
					//8 자리가 아니다
					continue;
				}
				if (get00000000byInt(s2) < 0)
				{
					//自然数でない
					//자연수가 아닌
					continue;
				}
				ps.Add(s2);
			}
            //Debug.Log("=== ps.toString ===");
            //Debug.Log(ps.ToString());
            //파일이 없을때
            if (ps.Count == 0)
			{
				return myError("連結対象のポーズ연결 대상의 포즈\n(" + anmName + "_00000000～)\nが見つかりません찾을 수 없습니다");
			}
			ps.Sort();
            //파일이 하나일때
			if (ps.Count == 1)
			{
				return myError("連結対象のポーズ연결 대상의 포즈\n(" + anmName + "_00000000～)\nが一つしかありません이 하나 밖에 없습니다");
			}
			else if (get00000000byInt(ps[0]) != 0)
			{
				return myError("最初のポーズ첫 번째 포즈(" + anmName + "_00000000)がありません이 없습니다");
			}
            Debug.Log("PoseStream3 ps.Count : " + ps.Count);
            if (minAnmMake)
            {
                // 중간 anm 생성
                if (makeAnmFile2(ps.ToArray()))
                {
                    Debug.Log("=== makeAnmFile2 ok ===" );                    
                    return "モーション모션「" + anmName + ".anm」を生成しました를 생성했습니다\nマイポーズのカテゴリをすでに表示している場合は\n一旦別のカテゴリを表示後にマイポーズを再表示してください\n마이뽀즈 범주를 이미보고있는 경우\n일단 다른 카테고리를 표시 후 마이뽀즈를 다시 표시하십시오";
                }
            }
            else
            {
                // 기존 anm생성
                if (makeAnmFile(ps.ToArray()))
                {
                    Debug.Log("=== makeAnmFile ok ===");
                    return "モーション모션「" + anmName + ".anm」を生成しました를 생성했습니다\nマイポーズのカテゴリをすでに表示している場合は\n一旦別のカテゴリを表示後にマイポーズを再表示してください\n마이뽀즈 범주를 이미보고있는 경우\n일단 다른 카테고리를 표시 후 마이뽀즈를 다시 표시하십시오";
                }
            }


			return myError(errorFile);
		}       

		public String errorFile = "";
        
        private bool makeAnmFile(String[] ss)
        {
            List<BoneDataA> bda = new List<BoneDataA>();
            byte[] header = new byte[15];
            bool isFirst = true;
            //読み込み
            //읽기
            foreach (String s in ss)
            {
                boneAdd(get00000000byInt(s), header, s, bda);
            }

            //結合
            //결합
            bool isExist = File.Exists(getPoseDataPath(true) + anmName + ".anm");
            using (BinaryWriter w = new BinaryWriter(File.Create(getPoseDataPath(true) + anmName + ".anm")))
            {
                try
                {
                    w.Write(header);
                    foreach (BoneDataA a in bda)
                    {
                        w.Write(a.outputABinary());
                    }
                    w.Write((byte)0);
                    w.Write((byte)0);
                    w.Write((byte)0);
                }
                catch (Exception)
                {
                    errorFile = "モーション「" + anmName + ".anm」の書き出し中にエラーが発生しました\n내보내기 중에 오류가 발생했습니다";
                    return false;
                }
            }
            if (!isExist)
            {
                MotionWindow mw = GameObject.FindObjectOfType<MotionWindow>();
                if (mw != null)
                {
                    PopupAndTabList patl = mw.PopupAndTabList;
                    try
                    {
                        mw.AddMyPose(getPoseDataPath(true) + anmName + @".anm");
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.ToString());
                    }
                }
            }

            return true;
        }



        // 본처리 단순화
        // BoneDataCadd(time, r, b);
        private void BoneDataCadd(int time, BinaryReader r, BoneDataB b)
        {
            BoneDataC bc = new BoneDataC();
            //Debug.Log("bc" + bc.ToString());
            bc.time = time;
            //time
            r.ReadBytes(4);
            //raw
            bc.raw = r.ReadBytes(12);
            byte[] raw4 = null;
            Array.Copy(bc.raw, raw4, 4);
            Debug.Log("BitConverter " + BitConverter.ToInt32(raw4, 0));
            
            b.c.Add(bc);
        }

        private void BoneDataCadd2(int time, BinaryReader r, BoneDataB b)
        {
            r.ReadBytes(4);
            byte[] raw3 = r.ReadBytes(12);
            byte[] raw4 = null;
            byte[] raw5 = null;
            Array.Copy(raw3, raw4, 4);
            Debug.Log("b.c.Count " + b.c.Count);
            
            //BoneDataC bc = new BoneDataC();
            for (int i= 0; i < b.c.Count;i++)
            {
                BoneDataC bc = b.c[i];
                //Debug.Log("bc" + bc.ToString());
                bc.time2 = time;
                //time
                
                //raw
                bc.raw2 = raw3;
                Array.Copy(bc.raw, raw5, 4);
                Debug.Log("BitConverter " + BitConverter.ToInt32(raw4, 0)+" / " + BitConverter.ToInt32(raw5, 0));
                bc.rawMid();
                //b.c.Add(bc);
            }

        }

        // 실제 파일 생성. 중간 생성
        private bool makeAnmFile2(String[] ss)
		{

            //読み込み
            //읽기
            for(int f=0;f<ss.Length - 1;f++)
            {
                // 본 데이터 목록 저장용
                
                String s = ss[f];
                String s2 = ss[f+1];
                Debug.Log("String s " + s+" s2 "+s2);
                // 해더
                byte[] header = new byte[15];

                // 첫번째 포즈                
                List<BoneDataA> bda = new List<BoneDataA>();

                Debug.Log("BinaryReader 1 ");
                // 첫번째 포즈 입력
                if (true) {
                    boneAdd( 0, header, s, bda);
                }

                Debug.Log("BinaryReader 2 ");
                // 두번째 포즈를 첫번째 포즈에 병합
                boneAdd(0, header,s2, bda);

                // 파일로 출력
                int si = get00000000byInt2(s, s2);
                Debug.Log("String si " + si);
                using (BinaryWriter w = new BinaryWriter(File.Create(getPoseDataPath(true) + anmName +"_" +si.ToString("D8") + ".anm")))
                {
                    try
                    {
                        w.Write(header);
                        foreach (BoneDataA a in bda)
                        {
                            w.Write(a.outputABinary());
                        }
                        w.Write((byte)0);
                        w.Write((byte)0);
                        w.Write((byte)0);
                    }
                    catch (Exception)
                    {
                        errorFile = "モーション「" + anmName + ".anm」の書き出し中にエラーが発生しました\n내보내기 중에 오류가 발생했습니다";
                        return false;
                    }
                }
            }

            //結合
            //결합
            bool isExist = File.Exists(getPoseDataPath(true) + anmName + ".anm");

            if (!isExist)
            {
                MotionWindow mw = GameObject.FindObjectOfType<MotionWindow>();
                if (mw != null)
                {
                    PopupAndTabList patl = mw.PopupAndTabList;
                    try
                    {
                        mw.AddMyPose(getPoseDataPath(true) + anmName + @".anm");
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.ToString());
                    }
                }
            }

            return true;
        }


        private void boneAdd(int time, byte[] header,string s, List<BoneDataA> bda)
        {
            using (BinaryReader r = new BinaryReader(File.OpenRead(getPoseDataPath(true) + s)))
            {
                //int time = get00000000byInt(s);
                //最初以外はヘッダー部分を読み飛ばす
                //첫 이외는 헤더 부분을 건너
                //r.ReadBytes(15);
                for (int i = 0; i < 15; i++)
                {
                    header[i] = r.ReadByte();
                }
                byte t = r.ReadByte();
                while (true)
                {
                    if (t == 1)
                    {
                        //A先頭部分
                        //A 선두 부분
                        t = r.ReadByte();
                        byte c = 0;
                        String name="";
                        bool isB = false;
                        c = r.ReadByte();
                        if (c == 1)
                        {
                            isB = true;
                            name = "";
                        }
                        else
                        {
                            isB = false;
                            name = "" + (char)c;
                            t--;
                        }
                        for (int i = 0; i < t; i++)
                        {
                            c = r.ReadByte();
                            name += (char)c;
                        }
                        Debug.Log("name "+ name);
                        BoneDataA a = null;
                        foreach (BoneDataA tmp in bda)
                        {
                            if (tmp.name.Equals(name))
                            {
                                a = tmp;
                                break;
                            }
                        }

                        // BoneDataA 본이 없는경우 추가
                        if (a == null)
                        {
                            Debug.Log("BoneDataA false");
                            a = new BoneDataA();
                            a.name = name;
                            a.isB = isB;
                            a.b = new List<BoneDataB>();
                            //B部分
                            //파트 B
                            while (true)
                            {
                                t = r.ReadByte();
                                if (t >= 64)
                                {
                                    BoneDataB b = new BoneDataB();
                                    b.index = t;
                                    int tmpf = r.ReadByte();
                                    r.ReadByte();
                                    r.ReadByte();
                                    r.ReadByte();
                                    //C部分
                                    bool firstFrame = true;
                                    for (int i = 0; i < tmpf; i++)
                                    {
                                        if (firstFrame)
                                        {
                                            firstFrame = false;
                                            b.c = new List<BoneDataC>();

                                            BoneDataCadd(0, r, b);
                                        }
                                        else
                                        {                                            
                                            BoneDataCadd(time, r, b);
                                        }
                                    }
                                    a.b.Add(b);
                                }
                                else
                                {
                                    break;
                                }
                            }
                            bda.Add(a);
                        }
                        // BoneDataA 본이 있는경우
                        else
                        {
                            Debug.Log("BoneDataA true");
                            //B部分
                            while (true)
                            {
                                t = r.ReadByte();
                                if (t >= 64)
                                {
                                    BoneDataB b = null;
                                    foreach (BoneDataB tmpb in a.b)
                                    {
                                        if (t == tmpb.index)
                                        {
                                            b = tmpb;
                                            break;
                                        }
                                    }
                                    // BoneDataB본이 없는경우 기존대로 추가
                                    Debug.Log(name + " / " + b.index + " / c " + b.c.Count);
                                    if (b == null)
                                    {
                                        b = new BoneDataB();
                                        b.index = t;
                                        int tmpf = r.ReadByte();
                                        r.ReadByte();
                                        r.ReadByte();
                                        r.ReadByte();
                                        //C部分
                                        bool firstFrame = true;
                                        for (int i = 0; i < tmpf; i++)
                                        {
                                            if (firstFrame)
                                            {
                                                firstFrame = false;
                                                b.c = new List<BoneDataC>();

                                                BoneDataCadd(0, r, b);
                                            }
                                            else
                                            {
                                                BoneDataCadd(time, r, b);
                                            }
                                        }
                                        a.b.Add(b);
                                    }
                                    // BoneDataB본이 있는경우 중간값 처리
                                    else
                                    {
                                        int tmpf = r.ReadByte();
                                        r.ReadByte();
                                        r.ReadByte();
                                        r.ReadByte();
                                        //C部分
                                        bool firstFrame = true;
                                        for (int i = 0; i < tmpf; i++)
                                        {
                                            if (firstFrame)
                                            {
                                                firstFrame = false;

                                                BoneDataCadd(time, r, b);
                                            }
                                            else
                                            {
                                                r.ReadBytes(16);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }

                    }
                    else
                    {
                        break;
                    }
                }
            }

        }

		private class BoneDataA
		{
			public String name = "";
			public bool isB = false;
			public List<BoneDataB> b = null;
			public byte[] outputABinary()
			{
				List<byte> lb = new List<byte>();
				//01
				lb.Add(1);
				//(ボーン名の長さ)
				//(본 이름의 길이)
				lb.Add((byte)(name.Length));
				//[01](長いボーン名だとある？)
				//[01](긴 뼈 이름이라고있다?)
				if (isB)
				{
					lb.Add(1);
				}
				//ボーン名
				char[] c = name.ToCharArray();
				for (int i = 0; i < name.Length; i++)
				{
					lb.Add((byte)(c[i]));
				}
				//B
				foreach(BoneDataB b1 in b)
				{
					lb.AddRange(b1.outputBBinary());
				}
				return lb.ToArray();
			}

		}
		private class BoneDataB
		{
			public int index = 0;
			public List<BoneDataC> c = null;
			public byte[] outputBBinary()
			{
				List<byte> lb = new List<byte>();
				//XX(64から始まる)
				//XX(64에서 시작)
				lb.Add((byte)index);
				//XX XX XX XX(フレーム数)
				//XX XX XX XX(프레임 수)
				lb.Add((byte)((c.Count) % 256));
				lb.Add((byte)(((c.Count) / 256) % 256));
				lb.Add((byte)((((c.Count) / 256) / 256) % 256));
				lb.Add((byte)(((((c.Count) / 256) / 256) / 256) % 256));
				//C
				foreach (BoneDataC c1 in c)
				{
					lb.AddRange(c1.outputCBinary());
				}
				return lb.ToArray();
			}
		}
		private class BoneDataC
		{
			public int time = 0;
			public int time2 = 0;
			public byte[] raw = null; //12바이트 실제 포즈값인듯
			public byte[] raw2 = null;
			public byte[] outputCBinary()
			{
				List<byte> lb = new List<byte>();
				float f = time / 1000f;
				byte[] b = new byte[4];
				byte[] t = BitConverter.GetBytes(f);
				b[0] = t[0];
				b[1] = t[1];
				b[2] = t[2];
				b[3] = t[3];
				//時間
				//시간
				lb.AddRange(b);
				//データ
				//데이터
				lb.AddRange(raw);
                //Debug.Log("BoneDataC lb " + lb.ToString());
                return lb.ToArray();
			}
            public void rawMid()
			{
                for(int i=0; i < raw.Length; i++)
                {
                    //Debug.Log("raw " + i + " / " + raw[i] + " / " + raw2[i]);
                    raw[i] = (byte)((raw[i] + raw2[i]) / 2);                    
                }
			}


		}

		private int get00000000byInt(String s)
		{
			String cut = s.Substring(anmName.Length + 1, 8);
			int t = 0;
			try
			{
				for (int i = 0; i < 8; i++)
				{
					t = t * 10 + int.Parse(cut.Substring(i, 1));
				}
			}
			catch (Exception)
			{
				t = -1;
			}
			return t;
		}

		private int get00000000byInt2(String s,String s2)
		{
			String cut = s.Substring(anmName.Length + 1, 8);
			String cut2 = s2.Substring(anmName.Length + 1, 8);
			int t = 0;
			int t2 = 0;
			try
			{
				for (int i = 0; i < 8; i++)
				{
					t = t * 10 + int.Parse(cut.Substring(i, 1));
				}
                for (int i = 0; i < 8; i++)
				{
					t2 = t2 * 10 + int.Parse(cut2.Substring(i, 1));
				}
                t = (t + t2) / 2;
			}
			catch (Exception)
			{
				t = -1;
			}
			return t;
		}

		private String myError(String s)
		{
			return "エラー오류:" + s;
		}

		private void onGUIFunc(int winId)
		{
			GUIStyle gsTextFiled = new GUIStyle(GUI.skin.textField);
			gsTextFiled.fontSize = getGUIparam(GUIPARAM.S);
			anmName = GUI.TextField(new Rect(getGUIparam(GUIPARAM.TX), getGUIparam(GUIPARAM.TY), getGUIparam(GUIPARAM.TW), getGUIparam(GUIPARAM.TH)), anmName, 30, gsTextFiled);
			GUIStyle gsLabel = new GUIStyle(GUI.skin.label);
			gsLabel.fontSize = getGUIparam(GUIPARAM.S);
			GUI.Label(new Rect(getGUIparam(GUIPARAM.LX), getGUIparam(GUIPARAM.LY), getGUIparam(GUIPARAM.LW), getGUIparam(GUIPARAM.LH)), "名前（*_00000000の*の部分）を入力してください\nすでに生成先ファイルが存在する場合は上書きされます\n이름(*_00000000의*부분)을 입력하십시오. \n 이미 생성 된 파일이 있으면 덮어 씁니다.test_00000000파일인 경우 test입력", gsLabel);
			GUIStyle gsButton2 = new GUIStyle(GUI.skin.button);
			gsButton2.fontSize = getGUIparam(GUIPARAM.S) * 2;
			gsButton2.alignment = TextAnchor.MiddleCenter;
			if (GUI.Button(new Rect(getGUIparam(GUIPARAM.XX), getGUIparam(GUIPARAM.XY), getGUIparam(GUIPARAM.XW), getGUIparam(GUIPARAM.XH)), "×", gsButton2))
			{
				isGUI = false;
				resultMessage = "";
			}
			GUIStyle gsButton = new GUIStyle(GUI.skin.button);
			gsButton.fontSize = getGUIparam(GUIPARAM.S);
			gsButton.alignment = TextAnchor.MiddleLeft;
            // 생성 버튼 클릭시
			if (GUI.Button(new Rect(getGUIparam(GUIPARAM.EXEBX), getGUIparam(GUIPARAM.EXEBY), getGUIparam(GUIPARAM.EXEBW), getGUIparam(GUIPARAM.EXEBH)), "生成생성", gsButton))
			{
                // anmMake 호출
                resultMessage = anmMake(false);
			}
            GUIStyle gsButton3 = new GUIStyle(GUI.skin.button);
            gsButton3.fontSize = getGUIparam(GUIPARAM.S);
            gsButton3.alignment = TextAnchor.MiddleLeft;
            // 생성 버튼 클릭시
            if (GUI.Button(new Rect(getGUIparam(GUIPARAM.EXEBX ) + getGUIparam(GUIPARAM.EXEBW), getGUIparam(GUIPARAM.EXEBY), getGUIparam(GUIPARAM.EXEBW), getGUIparam(GUIPARAM.EXEBH)), "test", gsButton3))
			{
                // anmMake 호출
                resultMessage = anmMake(true);
			}
			GUI.Label(new Rect(getGUIparam(GUIPARAM.RX), getGUIparam(GUIPARAM.RY), getGUIparam(GUIPARAM.RW), getGUIparam(GUIPARAM.RH)), resultMessage, gsLabel);
		}

		public void OnGUI()
		{
			try
			{
                //포토모드이고 ui가 떠있을시
				if ( isGUI)
				//if (isStudio && isGUI)
				{
                    //Debug.Log(Label);
					GUIStyle gsWin = new GUIStyle(GUI.skin.box);
					gsWin.fontSize = getGUIparam(GUIPARAM.S);
					gsWin.alignment = TextAnchor.UpperLeft;
					GUI.Window(200, new Rect(getGUIparam(GUIPARAM.X), getGUIparam(GUIPARAM.Y), getGUIparam(GUIPARAM.W), getGUIparam(GUIPARAM.H)), onGUIFunc, Label, gsWin);
				}
			}
			catch (Exception e)
			{
				Debug.LogError(e.ToString());
			}
		}

		private void Initialize()
		{
			try
			{
			}
			catch (Exception e)
			{
				Debug.LogError(e.ToString());
			}
		}
		
		
	}
	#endregion
}
