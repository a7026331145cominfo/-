# Native screen-link evidence extracted from historical source reports

Generated from the historical archive in commit 3a9492c85ca92a03d9a2b696ec8d351f380526bd.
This is evidence for implementation decisions, not a claim that the original project files were present as loose C# files.

- Extracted files: 40
- Loose C# files: 0
- Loose project files: 0

## Report inventory

- __unzipped__/╪к┘В╪з╪▒┘К╪▒/00_REAL_INTERNAL_UI_DESIGN.txt — 110,666 bytes
- __unzipped__/╪к┘В╪з╪▒┘К╪▒/01_ORIGINAL_PROJECT_TREE.txt — 274,509 bytes
- __unzipped__/╪к┘В╪з╪▒┘К╪▒/03_ORIGINAL_SECURITY_TVP_FILES.txt — 3,583 bytes
- __unzipped__/╪к┘В╪з╪▒┘К╪▒/04_ORIGINAL_ERP_MODULE_FILES.txt — 101,789 bytes
- __unzipped__/╪к┘В╪з╪▒┘К╪▒/05_CORE_SOURCE_REPORT.txt — 9,283,296 bytes
- __unzipped__/╪к┘В╪з╪▒┘К╪▒/06_SALES_PURCHASE_CORE.txt — 1,297,710 bytes
- __unzipped__/╪к┘В╪з╪▒┘К╪▒/AlSaqar_Internal_Source_Map.txt — 51,203,572 bytes
- __unzipped__/╪к┘В╪з╪▒┘К╪▒/GTSErp_SOURCE_MAP.txt — 31,022 bytes
- __unzipped__/╪к┘В╪з╪▒┘К╪▒/MajedSoft-TRUE-Screen-Mapping.txt — 505,522 bytes
- __unzipped__/╪к┘В╪з╪▒┘К╪▒/MajedSoft_Build_Report.txt — 361,548 bytes

## Screen metadata and access checks

### 01_ORIGINAL_PROJECT_TREE.txt — line 1338

1336: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\User_Groups.cs                                                          870 02/10/26 07:37:04 م .cs      
1337: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\User_Login.cs                                                          1233 02/10/26 07:37:04 م .cs      
1338: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\User_Permission.cs                                                      871 02/10/26 07:37:04 م .cs      
1339: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\User_Screens.cs                                                         302 02/10/26 07:37:04 م .cs      
1340: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\User_SellPrice.cs                                                       129 02/10/26 07:37:04 م .cs      

### 01_ORIGINAL_PROJECT_TREE.txt — line 1339

1337: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\User_Login.cs                                                          1233 02/10/26 07:37:04 م .cs      
1338: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\User_Permission.cs                                                      871 02/10/26 07:37:04 م .cs      
1339: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\User_Screens.cs                                                         302 02/10/26 07:37:04 م .cs      
1340: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\User_SellPrice.cs                                                       129 02/10/26 07:37:04 م .cs      
1341: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\View_AccountsLastLeve.cs                                               1259 02/10/26 07:37:04 م .cs      

### 03_ORIGINAL_SECURITY_TVP_FILES.txt — line 19

17: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\Select_Scaffold_ExitPermission_Result.cs              955
18: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\User_Login.cs                                        1233
19: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\User_Permission.cs                                    871
20: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\User_Screens.cs                                       302
21: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\BLL\Contract\Class_ScaffEntryPermission.cs          43229

### 03_ORIGINAL_SECURITY_TVP_FILES.txt — line 20

18: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\User_Login.cs                                        1233
19: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\User_Permission.cs                                    871
20: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\User_Screens.cs                                       302
21: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\BLL\Contract\Class_ScaffEntryPermission.cs          43229
22: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\BLL\Contract\Class_ScaffExitPermission.cs           43106

### 05_CORE_SOURCE_REPORT.txt — line 90

88: 	public void DeletePermission(int GPID)
89: 	{
90: 		List<User_Permission> list = ((IQueryable<User_Permission>)db.User_Permission).Where((User_Permission x) => x.GroupID == (int?)GPID).ToList();
91: 		if (list == null)
92: 		{

### 05_CORE_SOURCE_REPORT.txt — line 95

93: 			return;
94: 		}
95: 		foreach (User_Permission item in list)
96: 		{
97: 			db.User_Permission.Attach(item);

### 05_CORE_SOURCE_REPORT.txt — line 97

95: 		foreach (User_Permission item in list)
96: 		{
97: 			db.User_Permission.Attach(item);
98: 			db.User_Permission.Remove(item);
99: 		}

### 05_CORE_SOURCE_REPORT.txt — line 98

96: 		{
97: 			db.User_Permission.Attach(item);
98: 			db.User_Permission.Remove(item);
99: 		}
100: 		((DbContext)db).SaveChanges();

### 05_CORE_SOURCE_REPORT.txt — line 135

133: 	}
134: 
135: 	public List<User_Screens> GetAllGroupsPermaion()
136: 	{
137: 		return ((IQueryable<User_Screens>)db.User_Screens).Where((User_Screens x) => x.ISShow == (bool?)true).ToList();

### 05_CORE_SOURCE_REPORT.txt — line 137

135: 	public List<User_Screens> GetAllGroupsPermaion()
136: 	{
137: 		return ((IQueryable<User_Screens>)db.User_Screens).Where((User_Screens x) => x.ISShow == (bool?)true).ToList();
138: 	}
139: 

### 05_CORE_SOURCE_REPORT.txt — line 140

138: 	}
139: 
140: 	public List<User_Permission> GetPermassionByGroupID(int GroupId)
141: 	{
142: 		return ((IQueryable<User_Permission>)db.User_Permission).Where((User_Permission x) => x.GroupID == (int?)GroupId).ToList();

### 05_CORE_SOURCE_REPORT.txt — line 142

140: 	public List<User_Permission> GetPermassionByGroupID(int GroupId)
141: 	{
142: 		return ((IQueryable<User_Permission>)db.User_Permission).Where((User_Permission x) => x.GroupID == (int?)GroupId).ToList();
143: 	}
144: 

### 06_SALES_PURCHASE_CORE.txt — line 4544

4542: GTSErpSystem\User_Login.cs:19: public int? UserID_Add { get; set; }
4543: GTSErpSystem\User_Login.cs:27: public int? UserID_Update { get; set; }
4544: GTSErpSystem\User_Permission.cs:5: public class User_Permission
4545: GTSErpSystem\User_Permission.cs:11: public int? GroupID { get; set; }
4546: GTSErpSystem\User_Permission.cs:27: public int? UserID_Add { get; set; }

### 06_SALES_PURCHASE_CORE.txt — line 4545

4543: GTSErpSystem\User_Login.cs:27: public int? UserID_Update { get; set; }
4544: GTSErpSystem\User_Permission.cs:5: public class User_Permission
4545: GTSErpSystem\User_Permission.cs:11: public int? GroupID { get; set; }
4546: GTSErpSystem\User_Permission.cs:27: public int? UserID_Add { get; set; }
4547: GTSErpSystem\User_Permission.cs:35: public int? UserID_Update { get; set; }

### 06_SALES_PURCHASE_CORE.txt — line 4546

4544: GTSErpSystem\User_Permission.cs:5: public class User_Permission
4545: GTSErpSystem\User_Permission.cs:11: public int? GroupID { get; set; }
4546: GTSErpSystem\User_Permission.cs:27: public int? UserID_Add { get; set; }
4547: GTSErpSystem\User_Permission.cs:35: public int? UserID_Update { get; set; }
4548: GTSErpSystem\User_Screens.cs:3: public class User_Screens

### 06_SALES_PURCHASE_CORE.txt — line 4547

4545: GTSErpSystem\User_Permission.cs:11: public int? GroupID { get; set; }
4546: GTSErpSystem\User_Permission.cs:27: public int? UserID_Add { get; set; }
4547: GTSErpSystem\User_Permission.cs:35: public int? UserID_Update { get; set; }
4548: GTSErpSystem\User_Screens.cs:3: public class User_Screens
4549: GTSErpSystem\ViewAccountsLastLeve.cs:45: public int? UserID_Add { get; set; }

### 06_SALES_PURCHASE_CORE.txt — line 4548

4546: GTSErpSystem\User_Permission.cs:27: public int? UserID_Add { get; set; }
4547: GTSErpSystem\User_Permission.cs:35: public int? UserID_Update { get; set; }
4548: GTSErpSystem\User_Screens.cs:3: public class User_Screens
4549: GTSErpSystem\ViewAccountsLastLeve.cs:45: public int? UserID_Add { get; set; }
4550: GTSErpSystem\ViewAccountsLastLeve.cs:47: public int? UserID_Update { get; set; }

### 06_SALES_PURCHASE_CORE.txt — line 10191

10189: GTSErpSystem\User_Login.cs:19: public int? UserID_Add { get; set; }
10190: GTSErpSystem\User_Login.cs:27: public int? UserID_Update { get; set; }
10191: GTSErpSystem\User_Permission.cs:5: public class User_Permission
10192: GTSErpSystem\User_Permission.cs:11: public int? GroupID { get; set; }
10193: GTSErpSystem\User_Permission.cs:27: public int? UserID_Add { get; set; }

### 06_SALES_PURCHASE_CORE.txt — line 10192

10190: GTSErpSystem\User_Login.cs:27: public int? UserID_Update { get; set; }
10191: GTSErpSystem\User_Permission.cs:5: public class User_Permission
10192: GTSErpSystem\User_Permission.cs:11: public int? GroupID { get; set; }
10193: GTSErpSystem\User_Permission.cs:27: public int? UserID_Add { get; set; }
10194: GTSErpSystem\User_Permission.cs:35: public int? UserID_Update { get; set; }

### 06_SALES_PURCHASE_CORE.txt — line 10193

10191: GTSErpSystem\User_Permission.cs:5: public class User_Permission
10192: GTSErpSystem\User_Permission.cs:11: public int? GroupID { get; set; }
10193: GTSErpSystem\User_Permission.cs:27: public int? UserID_Add { get; set; }
10194: GTSErpSystem\User_Permission.cs:35: public int? UserID_Update { get; set; }
10195: GTSErpSystem\User_Screens.cs:3: public class User_Screens

### AlSaqar_Internal_Source_Map.txt — line 1348

1346: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\User_Groups.cs | 870 bytes
1347: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\User_Login.cs | 1233 bytes
1348: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\User_Permission.cs | 871 bytes
1349: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\User_Screens.cs | 302 bytes
1350: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\User_SellPrice.cs | 129 bytes

### AlSaqar_Internal_Source_Map.txt — line 1349

1347: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\User_Login.cs | 1233 bytes
1348: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\User_Permission.cs | 871 bytes
1349: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\User_Screens.cs | 302 bytes
1350: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\User_SellPrice.cs | 129 bytes
1351: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\View_AccountsLastLeve.cs | 1259 bytes

### AlSaqar_Internal_Source_Map.txt — line 3849

3847:   LINE 5: public class User_Login
3848: 
3849: FILE: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\User_Permission.cs
3850:   LINE 5: public class User_Permission
3851: 

### AlSaqar_Internal_Source_Map.txt — line 3850

3848: 
3849: FILE: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\User_Permission.cs
3850:   LINE 5: public class User_Permission
3851: 
3852: FILE: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\User_Screens.cs

### AlSaqar_Internal_Source_Map.txt — line 3852

3850:   LINE 5: public class User_Permission
3851: 
3852: FILE: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\User_Screens.cs
3853:   LINE 3: public class User_Screens
3854: 

### AlSaqar_Internal_Source_Map.txt — line 3853

3851: 
3852: FILE: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\User_Screens.cs
3853:   LINE 3: public class User_Screens
3854: 
3855: FILE: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\User_SellPrice.cs

### AlSaqar_Internal_Source_Map.txt — line 7977

7975: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\BLL\Restaurant\Class_Table.cs:77 | public string GetUnitName(int Code)
7976: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\BLL\Security\CheckPrvlg.cs:7 | private GTSdbEntities db = new GTSdbEntities();
7977: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\BLL\Security\CheckPrvlg.cs:9 | public bool CheckUserPage(int GroupID, int ScreenID)
7978: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\BLL\Security\CheckPrvlg.cs:17 | public bool CheckUserPageEventSave(int GroupID, int ScreenID)
7979: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\BLL\Security\CheckPrvlg.cs:25 | public bool CheckUserPageEventEdit(int GroupID, int ScreenID)

### AlSaqar_Internal_Source_Map.txt — line 7978

7976: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\BLL\Security\CheckPrvlg.cs:7 | private GTSdbEntities db = new GTSdbEntities();
7977: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\BLL\Security\CheckPrvlg.cs:9 | public bool CheckUserPage(int GroupID, int ScreenID)
7978: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\BLL\Security\CheckPrvlg.cs:17 | public bool CheckUserPageEventSave(int GroupID, int ScreenID)
7979: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\BLL\Security\CheckPrvlg.cs:25 | public bool CheckUserPageEventEdit(int GroupID, int ScreenID)
7980: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\BLL\Security\CheckPrvlg.cs:33 | public bool CheckUserPageEventDelete(int GroupID, int ScreenID)

### GTSErp_SOURCE_MAP.txt — line 839

837: \ufn_FindReports_Result.cs
838: \User_Login.cs
839: \User_Permission.cs
840: \View_AccountsLastLeve.cs
841: \View_Budget_Accounts.cs

### MajedSoft-TRUE-Screen-Mapping.txt — line 5189

5187: 21950: Class      : GTSErpSystem.User_Groups
5188: 21957: Class      : GTSErpSystem.User_Login
5189: 21964: Class      : GTSErpSystem.User_Permission
5190: 21971: Class      : GTSErpSystem.User_Screens
5191: 22027: Class      : GTSErpSystem.ViewAccount_Data

### MajedSoft-TRUE-Screen-Mapping.txt — line 5190

5188: 21957: Class      : GTSErpSystem.User_Login
5189: 21964: Class      : GTSErpSystem.User_Permission
5190: 21971: Class      : GTSErpSystem.User_Screens
5191: 22027: Class      : GTSErpSystem.ViewAccount_Data
5192: 22034: Class      : GTSErpSystem.ViewAccount_Data_Tree

### MajedSoft-TRUE-Screen-Mapping.txt — line 7405

7403: G:\ماجد سوفت\MajedSoft 04-08-2026\App\GTSErpSystem-Analysis\02-Classes.txt:21950: Class      : GTSErpSystem.User_Groups
7404: G:\ماجد سوفت\MajedSoft 04-08-2026\App\GTSErpSystem-Analysis\02-Classes.txt:21957: Class      : GTSErpSystem.User_Login
7405: G:\ماجد سوفت\MajedSoft 04-08-2026\App\GTSErpSystem-Analysis\02-Classes.txt:21964: Class      : GTSErpSystem.User_Permission
7406: G:\ماجد سوفت\MajedSoft 04-08-2026\App\GTSErpSystem-Analysis\02-Classes.txt:21971: Class      : GTSErpSystem.User_Screens
7407: G:\ماجد سوفت\MajedSoft 04-08-2026\App\GTSErpSystem-Analysis\03-Methods.txt:40: get_userId ->

### MajedSoft-TRUE-Screen-Mapping.txt — line 7406

7404: G:\ماجد سوفت\MajedSoft 04-08-2026\App\GTSErpSystem-Analysis\02-Classes.txt:21957: Class      : GTSErpSystem.User_Login
7405: G:\ماجد سوفت\MajedSoft 04-08-2026\App\GTSErpSystem-Analysis\02-Classes.txt:21964: Class      : GTSErpSystem.User_Permission
7406: G:\ماجد سوفت\MajedSoft 04-08-2026\App\GTSErpSystem-Analysis\02-Classes.txt:21971: Class      : GTSErpSystem.User_Screens
7407: G:\ماجد سوفت\MajedSoft 04-08-2026\App\GTSErpSystem-Analysis\03-Methods.txt:40: get_userId ->
7408: G:\ماجد سوفت\MajedSoft 04-08-2026\App\GTSErpSystem-Analysis\03-Methods.txt:72: get_AllowEdit -> System.Nullable`1[[System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]

### MajedSoft-TRUE-Screen-Mapping.txt — line 7857

7855: Screen / Menu
7856:     ↓
7857: User_Screens
7858:     ↓
7859: User_Permission / User_Groups

### MajedSoft-TRUE-Screen-Mapping.txt — line 7859

7857: User_Screens
7858:     ↓
7859: User_Permission / User_Groups
7860:     ↓
7861: BLL / DAL

### MajedSoft_Build_Report.txt — line 407

405: G:\ماجد سوفت\البرنامج\MajedSoft 08-07-2026\App\SourceCode\GTSErpSystem\User_Groups.cs                                                  
406: G:\ماجد سوفت\البرنامج\MajedSoft 08-07-2026\App\SourceCode\GTSErpSystem\User_Login.cs                                                   
407: G:\ماجد سوفت\البرنامج\MajedSoft 08-07-2026\App\SourceCode\GTSErpSystem\User_Permission.cs                                              
408: G:\ماجد سوفت\البرنامج\MajedSoft 08-07-2026\App\SourceCode\GTSErpSystem\User_Screens.cs                                                 
409: G:\ماجد سوفت\البرنامج\MajedSoft 08-07-2026\App\SourceCode\GTSErpSystem\User_SellPrice.cs                                               


## Native type resolution and form launching

### 05_CORE_SOURCE_REPORT.txt — line 235

233: GTSErpSystem\FrmLogin.cs:519: select x).ToList();
234: GTSErpSystem\FrmLogin.cs:532: select x).ToList();
235: GTSErpSystem\FrmLogin.cs:545: ((Form)(object)new FrmSqlConnection()).ShowDialog();
236: GTSErpSystem\FrmLogin.cs:656: this.BtnDelete = new System.Windows.Forms.Button();
237: GTSErpSystem\User_Groups.cs:35: public int? UserID_Update { get; set; }

### 05_CORE_SOURCE_REPORT.txt — line 3410

3408: ============================================================
3409: GTSErpSystem\FrmActivtion.cs:77: FrmSqlConnection frmSqlConnection = new FrmSqlConnection();
3410: GTSErpSystem\FrmLogin.cs:545: ((Form)(object)new FrmSqlConnection()).ShowDialog();
3411: GTSErpSystem\FrmLogin3.cs:991: ((Form)(object)new FrmSqlConnection()).ShowDialog();
3412: GTSErpSystem\Program.cs:54: Application.Run((Form)(object)new FrmSqlConnection());

### 05_CORE_SOURCE_REPORT.txt — line 3411

3409: GTSErpSystem\FrmActivtion.cs:77: FrmSqlConnection frmSqlConnection = new FrmSqlConnection();
3410: GTSErpSystem\FrmLogin.cs:545: ((Form)(object)new FrmSqlConnection()).ShowDialog();
3411: GTSErpSystem\FrmLogin3.cs:991: ((Form)(object)new FrmSqlConnection()).ShowDialog();
3412: GTSErpSystem\Program.cs:54: Application.Run((Form)(object)new FrmSqlConnection());
3413: GTSErpSystem\Frms\Account\FrmAccountTree.cs:338: NavDelete = new NavBarItem();

### 05_CORE_SOURCE_REPORT.txt — line 3415

3413: GTSErpSystem\Frms\Account\FrmAccountTree.cs:338: NavDelete = new NavBarItem();
3414: GTSErpSystem\Frms\Account\FrmCostCenterTree.cs:385: NavDelete = new NavBarItem();
3415: GTSErpSystem\Frms\FrmsHome\FrmActivation.cs:1729: ((Form)(object)new FrmSqlConnection()).ShowDialog();
3416: GTSErpSystem\Frms\FrmsHome\FrmBlue.cs:1558: ((Form)(object)new FrmSqlConnection()).ShowDialog();
3417: GTSErpSystem\Frms\FrmsHome\FrmGold.cs:1570: ((Form)(object)new FrmSqlConnection()).ShowDialog();

### 05_CORE_SOURCE_REPORT.txt — line 3416

3414: GTSErpSystem\Frms\Account\FrmCostCenterTree.cs:385: NavDelete = new NavBarItem();
3415: GTSErpSystem\Frms\FrmsHome\FrmActivation.cs:1729: ((Form)(object)new FrmSqlConnection()).ShowDialog();
3416: GTSErpSystem\Frms\FrmsHome\FrmBlue.cs:1558: ((Form)(object)new FrmSqlConnection()).ShowDialog();
3417: GTSErpSystem\Frms\FrmsHome\FrmGold.cs:1570: ((Form)(object)new FrmSqlConnection()).ShowDialog();
3418: GTSErpSystem\Frms\FrmsHome\FrmGoldPlus.cs:1847: ((Form)(object)new FrmSqlConnection()).ShowDialog();

### 05_CORE_SOURCE_REPORT.txt — line 3417

3415: GTSErpSystem\Frms\FrmsHome\FrmActivation.cs:1729: ((Form)(object)new FrmSqlConnection()).ShowDialog();
3416: GTSErpSystem\Frms\FrmsHome\FrmBlue.cs:1558: ((Form)(object)new FrmSqlConnection()).ShowDialog();
3417: GTSErpSystem\Frms\FrmsHome\FrmGold.cs:1570: ((Form)(object)new FrmSqlConnection()).ShowDialog();
3418: GTSErpSystem\Frms\FrmsHome\FrmGoldPlus.cs:1847: ((Form)(object)new FrmSqlConnection()).ShowDialog();
3419: GTSErpSystem\Frms\FrmsHome\FrmGoldPlusImportExport.cs:1715: ((Form)(object)new FrmSqlConnection()).ShowDialog();

### 05_CORE_SOURCE_REPORT.txt — line 3418

3416: GTSErpSystem\Frms\FrmsHome\FrmBlue.cs:1558: ((Form)(object)new FrmSqlConnection()).ShowDialog();
3417: GTSErpSystem\Frms\FrmsHome\FrmGold.cs:1570: ((Form)(object)new FrmSqlConnection()).ShowDialog();
3418: GTSErpSystem\Frms\FrmsHome\FrmGoldPlus.cs:1847: ((Form)(object)new FrmSqlConnection()).ShowDialog();
3419: GTSErpSystem\Frms\FrmsHome\FrmGoldPlusImportExport.cs:1715: ((Form)(object)new FrmSqlConnection()).ShowDialog();
3420: GTSErpSystem\Frms\FrmsHome\FrmGoldPlusTransportation.cs:1740: ((Form)(object)new FrmSqlConnection()).ShowDialog();

### 05_CORE_SOURCE_REPORT.txt — line 3419

3417: GTSErpSystem\Frms\FrmsHome\FrmGold.cs:1570: ((Form)(object)new FrmSqlConnection()).ShowDialog();
3418: GTSErpSystem\Frms\FrmsHome\FrmGoldPlus.cs:1847: ((Form)(object)new FrmSqlConnection()).ShowDialog();
3419: GTSErpSystem\Frms\FrmsHome\FrmGoldPlusImportExport.cs:1715: ((Form)(object)new FrmSqlConnection()).ShowDialog();
3420: GTSErpSystem\Frms\FrmsHome\FrmGoldPlusTransportation.cs:1740: ((Form)(object)new FrmSqlConnection()).ShowDialog();
3421: GTSErpSystem\Frms\FrmsHome\FrmLight2.cs:1484: ((Form)(object)new FrmSqlConnection()).ShowDialog();

### AlSaqar_Internal_Source_Map.txt — line 6890

6888: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\BLL\Main\Class_Setting.cs:300 | public int DeleteRem(int id)
6889: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\BLL\Main\Class_Setting.cs:312 | public int AddLoginLog(int userId, string macAddress, string appVersion, string databaseVersion)
6890: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\BLL\Main\Class_WaitFormFunc.cs:13 | public void Show()
6891: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\BLL\Main\Class_WaitFormFunc.cs:19 | public void Show(Form parent)
6892: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\BLL\Main\Class_WaitFormFunc.cs:25 | public void Close()

### AlSaqar_Internal_Source_Map.txt — line 6891

6889: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\BLL\Main\Class_Setting.cs:312 | public int AddLoginLog(int userId, string macAddress, string appVersion, string databaseVersion)
6890: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\BLL\Main\Class_WaitFormFunc.cs:13 | public void Show()
6891: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\BLL\Main\Class_WaitFormFunc.cs:19 | public void Show(Form parent)
6892: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\BLL\Main\Class_WaitFormFunc.cs:25 | public void Close()
6893: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\BLL\Main\Class_WaitFormFunc.cs:35 | private void LoadingProcess()

### AlSaqar_Internal_Source_Map.txt — line 22558

22556: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\Frms\Reports\Account\FrmRPLeadgerSupp.cs:356 | private void SearchAccount()
22557: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\Frms\Reports\Account\FrmRPLeadgerSupp.cs:377 | private void Print()
22558: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\Frms\Reports\Account\FrmRPLeadgerSupp.cs:424 | private void OpenForm()
22559: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\Frms\Reports\Account\FrmRPLeadgerSupp.cs:609 | private void MenuNew_Click(object sender, EventArgs e)
22560: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem\Frms\Reports\Account\FrmRPLeadgerSupp.cs:614 | private void MenuSave_Click(object sender, EventArgs e)

### AlSaqar_Internal_Source_Map.txt — line 41497

41495:   LINE 18250: <member name="M:DevExpress.XtraEditors.XtraForm.ResumeLayout(System.Boolean)">
41496:   LINE 18256: <member name="M:DevExpress.XtraEditors.XtraForm.ResumeRedraw">
41497:   LINE 18261: <member name="M:DevExpress.XtraEditors.XtraForm.ShowDialog(System.Windows.Forms.IWin32Window)">
41498:   LINE 18268: <member name="P:DevExpress.XtraEditors.XtraForm.ShowIcon">
41499:   LINE 18270: <para>Gets or sets whether the <see cref="T:DevExpress.XtraEditors.XtraForm"/> shows its icon.</para>

### AlSaqar_Internal_Source_Map.txt — line 44134

44132:   LINE 9910: <para>Initializes a new instance of the <see cref="T:DevExpress.XtraBars.Docking2010.Customization.FlyoutDialog"/> class with the specified settings.</para>
44133:   LINE 9912: <param name="owner">A Form that will own the newly created <see cref="T:DevExpress.XtraBars.Docking2010.Customization.FlyoutDialog"/>.</param>
44134:   LINE 9977: <member name="M:DevExpress.XtraBars.Docking2010.Customization.FlyoutDialog.Show(System.Windows.Forms.Form,System.String,System.String,System.Windows.Forms.MessageBoxButtons)">
44135:   LINE 9984: <param name="buttons">A MessageBoxButtons enumerator value that specifies what buttons this <see cref="T:DevExpress.XtraBars.Docking2010.Customization.FlyoutDialog"/> will display.</param>
44136:   LINE 9987: <member name="M:DevExpress.XtraBars.Docking2010.Customization.FlyoutDialog.Show(System.Windows.Forms.Form,System.String,System.String,System.Windows.Forms.MessageBoxButtons,System.Windows.Forms.MessageBoxDefaultButton)">

### AlSaqar_Internal_Source_Map.txt — line 44136

44134:   LINE 9977: <member name="M:DevExpress.XtraBars.Docking2010.Customization.FlyoutDialog.Show(System.Windows.Forms.Form,System.String,System.String,System.Windows.Forms.MessageBoxButtons)">
44135:   LINE 9984: <param name="buttons">A MessageBoxButtons enumerator value that specifies what buttons this <see cref="T:DevExpress.XtraBars.Docking2010.Customization.FlyoutDialog"/> will display.</param>
44136:   LINE 9987: <member name="M:DevExpress.XtraBars.Docking2010.Customization.FlyoutDialog.Show(System.Windows.Forms.Form,System.String,System.String,System.Windows.Forms.MessageBoxButtons,System.Windows.Forms.MessageBoxDefaultButton)">
44137:   LINE 9994: <param name="buttons">A MessageBoxButtons enumerator value that specifies what buttons this <see cref="T:DevExpress.XtraBars.Docking2010.Customization.FlyoutDialog"/> will display.</param>
44138:   LINE 9995: <param name="defaultButton">A MessageBoxDefaultButton enumeration value that specifies which <see cref="T:DevExpress.XtraBars.Docking2010.Customization.FlyoutDialog"/> button is the default one. A default button is considered as clicked when end-users press the Enter key as the <see cref="T:DevExpress.XtraBars.Docking2010.Customization.FlyoutDialog"/> pops up.</param>

### AlSaqar_Internal_Source_Map.txt — line 44139

44137:   LINE 9994: <param name="buttons">A MessageBoxButtons enumerator value that specifies what buttons this <see cref="T:DevExpress.XtraBars.Docking2010.Customization.FlyoutDialog"/> will display.</param>
44138:   LINE 9995: <param name="defaultButton">A MessageBoxDefaultButton enumeration value that specifies which <see cref="T:DevExpress.XtraBars.Docking2010.Customization.FlyoutDialog"/> button is the default one. A default button is considered as clicked when end-users press the Enter key as the <see cref="T:DevExpress.XtraBars.Docking2010.Customization.FlyoutDialog"/> pops up.</param>
44139:   LINE 9998: <member name="M:DevExpress.XtraBars.Docking2010.Customization.FlyoutDialog.Show(System.Windows.Forms.Form,System.String,System.Windows.Forms.Control,DevExpress.XtraBars.Docking2010.Views.WindowsUI.FlyoutProperties,System.Windows.Forms.MessageBoxButtons,System.Windows.Forms.MessageBoxDefaultButton)">
44140:   LINE 10006: <param name="buttons">A MessageBoxButtons enumerator value that specifies what buttons this <see cref="T:DevExpress.XtraBars.Docking2010.Customization.FlyoutDialog"/> will display.</param>
44141:   LINE 10007: <param name="defaultButton">A MessageBoxDefaultButton enumeration value that specifies which <see cref="T:DevExpress.XtraBars.Docking2010.Customization.FlyoutDialog"/> button is the default one. A default button is considered as clicked when end-users press the Enter key as the <see cref="T:DevExpress.XtraBars.Docking2010.Customization.FlyoutDialog"/> pops up.</param>

### AlSaqar_Internal_Source_Map.txt — line 44142

44140:   LINE 10006: <param name="buttons">A MessageBoxButtons enumerator value that specifies what buttons this <see cref="T:DevExpress.XtraBars.Docking2010.Customization.FlyoutDialog"/> will display.</param>
44141:   LINE 10007: <param name="defaultButton">A MessageBoxDefaultButton enumeration value that specifies which <see cref="T:DevExpress.XtraBars.Docking2010.Customization.FlyoutDialog"/> button is the default one. A default button is considered as clicked when end-users press the Enter key as the <see cref="T:DevExpress.XtraBars.Docking2010.Customization.FlyoutDialog"/> pops up.</param>
44142:   LINE 10010: <member name="M:DevExpress.XtraBars.Docking2010.Customization.FlyoutDialog.Show(System.Windows.Forms.Form,System.String,System.Windows.Forms.Control,System.Windows.Forms.MessageBoxButtons)">
44143:   LINE 10017: <param name="buttons">A MessageBoxButtons enumerator value that specifies what buttons this <see cref="T:DevExpress.XtraBars.Docking2010.Customization.FlyoutDialog"/> will display.</param>
44144:   LINE 10020: <member name="M:DevExpress.XtraBars.Docking2010.Customization.FlyoutDialog.Show(System.Windows.Forms.Form,System.String,System.Windows.Forms.Control,System.Windows.Forms.MessageBoxButtons,System.Windows.Forms.MessageBoxDefaultButton)">


## High-value operational form names

### 00_REAL_INTERNAL_UI_DESIGN.txt — line 690

688: CONTROLS: 0
689: 
690: SCREEN: FrmCashir
691: NAMESPACE: GTSErpSystem.Frms.Orders
692: TEXT: 

### 00_REAL_INTERNAL_UI_DESIGN.txt — line 694

692: TEXT: 
693: SIZE:  x 
694: FILE: Frms\Orders\FrmCashir.cs
695: CONTROLS: 0
696: 

### 00_REAL_INTERNAL_UI_DESIGN.txt — line 697

695: CONTROLS: 0
696: 
697: SCREEN: FrmCashirPharm
698: NAMESPACE: GTSErpSystem.Frms.Orders
699: TEXT: 

### 00_REAL_INTERNAL_UI_DESIGN.txt — line 701

699: TEXT: 
700: SIZE:  x 
701: FILE: Frms\Orders\FrmCashirPharm.cs
702: CONTROLS: 1
703:   [GTSErpSystem.Frms.Orders.FrmOpenDay.FrmOpenDay] frmOpenDay | Text=[] | Location=[] | Size=[] | Dock=[] | Anchor=[]

### 00_REAL_INTERNAL_UI_DESIGN.txt — line 705

703:   [GTSErpSystem.Frms.Orders.FrmOpenDay.FrmOpenDay] frmOpenDay | Text=[] | Location=[] | Size=[] | Dock=[] | Anchor=[]
704: 
705: SCREEN: FrmCashirRestaurant
706: NAMESPACE: GTSErpSystem.Frms.FrmRestaurant
707: TEXT: 

### 00_REAL_INTERNAL_UI_DESIGN.txt — line 709

707: TEXT: 
708: SIZE:  x 
709: FILE: Frms\FrmRestaurant\FrmCashirRestaurant.cs
710: CONTROLS: 4
711:   [Font] Font | Text=[] | Location=[] | Size=[] | Dock=[] | Anchor=[]

### 00_REAL_INTERNAL_UI_DESIGN.txt — line 716

714:   [TileItem] val | Text=[] | Location=[] | Size=[] | Dock=[] | Anchor=[]
715: 
716: SCREEN: FrmCashirWashing
717: NAMESPACE: GTSErpSystem.Frms.Orders
718: TEXT: 

### 00_REAL_INTERNAL_UI_DESIGN.txt — line 720

718: TEXT: 
719: SIZE:  x 
720: FILE: Frms\Orders\FrmCashirWashing.cs
721: CONTROLS: 0
722: 

### 01_ORIGINAL_PROJECT_TREE.txt — line 96

94: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmCostCenter.resx                                       52514 02/10/26 07:36:49 م .resx    
95: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmCostCenterTree.resx                                   48306 02/10/26 07:36:49 م .resx    
96: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmDailyRestrictions.resx                               120289 02/10/26 07:36:49 م .resx    
97: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmDailyRestrictionsTest.resx                           115082 02/10/26 07:36:49 م .resx    
98: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmDefualtAccount.resx                                   76316 02/10/26 07:36:49 م .resx    

### 01_ORIGINAL_PROJECT_TREE.txt — line 97

95: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmCostCenterTree.resx                                   48306 02/10/26 07:36:49 م .resx    
96: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmDailyRestrictions.resx                               120289 02/10/26 07:36:49 م .resx    
97: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmDailyRestrictionsTest.resx                           115082 02/10/26 07:36:49 م .resx    
98: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmDefualtAccount.resx                                   76316 02/10/26 07:36:49 م .resx    
99: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmDefualtCustomer.resx                                  63276 02/10/26 07:36:49 م .resx    

### 01_ORIGINAL_PROJECT_TREE.txt — line 102

100: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmOpenAccount.resx                                      44546 02/10/26 07:36:49 م .resx    
101: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmPayment.resx                                          88665 02/10/26 07:36:49 م .resx    
102: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmPaymentBig.resx                                      138106 02/10/26 07:36:49 م .resx    
103: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmProjects.resx                                         78070 02/10/26 07:36:49 م .resx    
104: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmReceipts.resx                                         92023 02/10/26 07:36:49 م .resx    

### 01_ORIGINAL_PROJECT_TREE.txt — line 105

103: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmProjects.resx                                         78070 02/10/26 07:36:49 م .resx    
104: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmReceipts.resx                                         92023 02/10/26 07:36:49 م .resx    
105: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmReceiptsBig.resx                                     140207 02/10/26 07:36:49 م .resx    
106: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmReceiptsScaffolds.resx                                99298 02/10/26 07:36:50 م .resx    
107: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.Search.FrmSearchAccount.resx                             21963 02/10/26 07:36:49 م .resx    

### 01_ORIGINAL_PROJECT_TREE.txt — line 165

163: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.ElctorncInvoic.SentInvoiceToZakat.resx                            5952 02/10/26 07:36:49 م .resx    
164: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.FrmOthers.FrmRepair.resx                                          4818 02/10/26 07:36:49 م .resx    
165: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.FrmRestaurant.FrmCashirRestaurant.resx                          326001 02/10/26 07:36:49 م .resx    
166: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.FrmRestaurant.FrmDeliveryCollecting.resx                          6171 02/10/26 07:36:49 م .resx    
167: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.FrmRestaurant.FrmEditRoom.resx                                    4194 02/10/26 07:36:49 م .resx    

### 01_ORIGINAL_PROJECT_TREE.txt — line 242

240: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.OrderManufacturing.FrmSearch.FrmSearchManufacturing.resx         14536 02/10/26 07:36:50 م .resx    
241: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.OrderManufacturing.FrmSearch.FrmSearchManufacturingOrder.resx    14542 02/10/26 07:36:50 م .resx    
242: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.OrderRent.FrmOrderReturnRental.resx                              17262 02/10/26 07:36:50 م .resx    
243: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.OrderRent.FrmRecipt.resx                                         19809 02/10/26 07:36:50 م .resx    
244: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.OrderRent.FrmRecipt2.resx                                        18909 02/10/26 07:36:50 م .resx    

### 01_ORIGINAL_PROJECT_TREE.txt — line 266

264: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Orders.FrmCachMoney.FrmMoneyCashier.resx                         82234 02/10/26 07:36:50 م .resx    
265: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Orders.FrmCars.resx                                               3408 02/10/26 07:36:50 م .resx    
266: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Orders.FrmCashir.resx                                           577773 02/10/26 07:36:50 م .resx    
267: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Orders.FrmCashirPharm.resx                                       19174 02/10/26 07:36:50 م .resx    
268: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Orders.FrmCashirWashing.resx                                    408207 02/10/26 07:36:50 م .resx    

### 01_ORIGINAL_PROJECT_TREE.txt — line 267

265: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Orders.FrmCars.resx                                               3408 02/10/26 07:36:50 م .resx    
266: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Orders.FrmCashir.resx                                           577773 02/10/26 07:36:50 م .resx    
267: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Orders.FrmCashirPharm.resx                                       19174 02/10/26 07:36:50 م .resx    
268: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Orders.FrmCashirWashing.resx                                    408207 02/10/26 07:36:50 م .resx    
269: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Orders.FrmCashirWithOutGroup.resx                               426010 02/10/26 07:36:50 م .resx    

### 04_ORIGINAL_ERP_MODULE_FILES.txt — line 16

14: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmCostCenter.resx                                     52514
15: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmCostCenterTree.resx                                 48306
16: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmDailyRestrictions.resx                             120289
17: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmDailyRestrictionsTest.resx                         115082
18: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmDefualtAccount.resx                                 76316

### 04_ORIGINAL_ERP_MODULE_FILES.txt — line 17

15: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmCostCenterTree.resx                                 48306
16: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmDailyRestrictions.resx                             120289
17: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmDailyRestrictionsTest.resx                         115082
18: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmDefualtAccount.resx                                 76316
19: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmDefualtCustomer.resx                                63276

### 04_ORIGINAL_ERP_MODULE_FILES.txt — line 22

20: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmOpenAccount.resx                                    44546
21: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmPayment.resx                                        88665
22: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmPaymentBig.resx                                    138106
23: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmProjects.resx                                       78070
24: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmReceipts.resx                                       92023

### 04_ORIGINAL_ERP_MODULE_FILES.txt — line 25

23: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmProjects.resx                                       78070
24: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmReceipts.resx                                       92023
25: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmReceiptsBig.resx                                   140207
26: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmReceiptsScaffolds.resx                              99298
27: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.Search.FrmSearchAccount.resx                           21963

### 04_ORIGINAL_ERP_MODULE_FILES.txt — line 73

71: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.OrderManufacturing.FrmSearch.FrmSearchManufacturing.resx       14536
72: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.OrderManufacturing.FrmSearch.FrmSearchManufacturingOrder.resx  14542
73: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.OrderRent.FrmOrderReturnRental.resx                            17262
74: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.OrderRent.FrmRecipt.resx                                       19809
75: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.OrderRent.FrmRecipt2.resx                                      18909

### 04_ORIGINAL_ERP_MODULE_FILES.txt — line 97

95: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Orders.FrmCachMoney.FrmMoneyCashier.resx                       82234
96: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Orders.FrmCars.resx                                             3408
97: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Orders.FrmCashir.resx                                         577773
98: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Orders.FrmCashirPharm.resx                                     19174
99: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Orders.FrmCashirWashing.resx                                  408207

### 04_ORIGINAL_ERP_MODULE_FILES.txt — line 98

96: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Orders.FrmCars.resx                                             3408
97: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Orders.FrmCashir.resx                                         577773
98: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Orders.FrmCashirPharm.resx                                     19174
99: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Orders.FrmCashirWashing.resx                                  408207
100: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Orders.FrmCashirWithOutGroup.resx                             426010

### 04_ORIGINAL_ERP_MODULE_FILES.txt — line 99

97: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Orders.FrmCashir.resx                                         577773
98: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Orders.FrmCashirPharm.resx                                     19174
99: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Orders.FrmCashirWashing.resx                                  408207
100: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Orders.FrmCashirWithOutGroup.resx                             426010
101: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Orders.FrmCheckOut.resx                                       170023

### 05_CORE_SOURCE_REPORT.txt — line 561

559: GTSErpSystem\Frms\Account\FrmCostCenter.cs:368: if (CheckPermession.CheckUserPageEventEdit(LoginDetails.GroupID, 2))
560: GTSErpSystem\Frms\Account\FrmCostCenter.cs:419: if (CheckPermession.CheckUserPageEventDelete(LoginDetails.GroupID, 2))
561: GTSErpSystem\Frms\Account\FrmDailyRestrictions.cs:244: bool flag = CheckPermession.CheckUserPageEventEdit(LoginDetails.GroupID, 36);
562: GTSErpSystem\Frms\Account\FrmDailyRestrictions.cs:245: bool flag2 = CheckPermession.CheckUserPageEventDelete(LoginDetails.GroupID, 36);
563: GTSErpSystem\Frms\Account\FrmDailyRestrictions.cs:719: if (CheckPermession.CheckUserPageEventSave(LoginDetails.GroupID, 36))

### 05_CORE_SOURCE_REPORT.txt — line 562

560: GTSErpSystem\Frms\Account\FrmCostCenter.cs:419: if (CheckPermession.CheckUserPageEventDelete(LoginDetails.GroupID, 2))
561: GTSErpSystem\Frms\Account\FrmDailyRestrictions.cs:244: bool flag = CheckPermession.CheckUserPageEventEdit(LoginDetails.GroupID, 36);
562: GTSErpSystem\Frms\Account\FrmDailyRestrictions.cs:245: bool flag2 = CheckPermession.CheckUserPageEventDelete(LoginDetails.GroupID, 36);
563: GTSErpSystem\Frms\Account\FrmDailyRestrictions.cs:719: if (CheckPermession.CheckUserPageEventSave(LoginDetails.GroupID, 36))
564: GTSErpSystem\Frms\Account\FrmDailyRestrictions.cs:779: if (CheckPermession.CheckUserPageEventEdit(LoginDetails.GroupID, 36))

### 05_CORE_SOURCE_REPORT.txt — line 563

561: GTSErpSystem\Frms\Account\FrmDailyRestrictions.cs:244: bool flag = CheckPermession.CheckUserPageEventEdit(LoginDetails.GroupID, 36);
562: GTSErpSystem\Frms\Account\FrmDailyRestrictions.cs:245: bool flag2 = CheckPermession.CheckUserPageEventDelete(LoginDetails.GroupID, 36);
563: GTSErpSystem\Frms\Account\FrmDailyRestrictions.cs:719: if (CheckPermession.CheckUserPageEventSave(LoginDetails.GroupID, 36))
564: GTSErpSystem\Frms\Account\FrmDailyRestrictions.cs:779: if (CheckPermession.CheckUserPageEventEdit(LoginDetails.GroupID, 36))
565: GTSErpSystem\Frms\Account\FrmDailyRestrictions.cs:850: if (CheckPermession.CheckUserPageEventDelete(LoginDetails.GroupID, 36))

### 05_CORE_SOURCE_REPORT.txt — line 564

562: GTSErpSystem\Frms\Account\FrmDailyRestrictions.cs:245: bool flag2 = CheckPermession.CheckUserPageEventDelete(LoginDetails.GroupID, 36);
563: GTSErpSystem\Frms\Account\FrmDailyRestrictions.cs:719: if (CheckPermession.CheckUserPageEventSave(LoginDetails.GroupID, 36))
564: GTSErpSystem\Frms\Account\FrmDailyRestrictions.cs:779: if (CheckPermession.CheckUserPageEventEdit(LoginDetails.GroupID, 36))
565: GTSErpSystem\Frms\Account\FrmDailyRestrictions.cs:850: if (CheckPermession.CheckUserPageEventDelete(LoginDetails.GroupID, 36))
566: GTSErpSystem\Frms\Account\FrmDailyRestrictions.cs:953: if (CheckPermession.CheckUserPage(LoginDetails.GroupID, 36))

### 05_CORE_SOURCE_REPORT.txt — line 565

563: GTSErpSystem\Frms\Account\FrmDailyRestrictions.cs:719: if (CheckPermession.CheckUserPageEventSave(LoginDetails.GroupID, 36))
564: GTSErpSystem\Frms\Account\FrmDailyRestrictions.cs:779: if (CheckPermession.CheckUserPageEventEdit(LoginDetails.GroupID, 36))
565: GTSErpSystem\Frms\Account\FrmDailyRestrictions.cs:850: if (CheckPermession.CheckUserPageEventDelete(LoginDetails.GroupID, 36))
566: GTSErpSystem\Frms\Account\FrmDailyRestrictions.cs:953: if (CheckPermession.CheckUserPage(LoginDetails.GroupID, 36))
567: GTSErpSystem\Frms\Account\FrmDailyRestrictions.cs:1397: if (CheckPermession.CheckUserPage(LoginDetails.GroupID, 139))

### 05_CORE_SOURCE_REPORT.txt — line 566

564: GTSErpSystem\Frms\Account\FrmDailyRestrictions.cs:779: if (CheckPermession.CheckUserPageEventEdit(LoginDetails.GroupID, 36))
565: GTSErpSystem\Frms\Account\FrmDailyRestrictions.cs:850: if (CheckPermession.CheckUserPageEventDelete(LoginDetails.GroupID, 36))
566: GTSErpSystem\Frms\Account\FrmDailyRestrictions.cs:953: if (CheckPermession.CheckUserPage(LoginDetails.GroupID, 36))
567: GTSErpSystem\Frms\Account\FrmDailyRestrictions.cs:1397: if (CheckPermession.CheckUserPage(LoginDetails.GroupID, 139))
568: GTSErpSystem\Frms\Account\FrmDailyRestrictionsTest.cs:691: if (CheckPermession.CheckUserPageEventSave(LoginDetails.GroupID, 35))

### 05_CORE_SOURCE_REPORT.txt — line 567

565: GTSErpSystem\Frms\Account\FrmDailyRestrictions.cs:850: if (CheckPermession.CheckUserPageEventDelete(LoginDetails.GroupID, 36))
566: GTSErpSystem\Frms\Account\FrmDailyRestrictions.cs:953: if (CheckPermession.CheckUserPage(LoginDetails.GroupID, 36))
567: GTSErpSystem\Frms\Account\FrmDailyRestrictions.cs:1397: if (CheckPermession.CheckUserPage(LoginDetails.GroupID, 139))
568: GTSErpSystem\Frms\Account\FrmDailyRestrictionsTest.cs:691: if (CheckPermession.CheckUserPageEventSave(LoginDetails.GroupID, 35))
569: GTSErpSystem\Frms\Account\FrmDailyRestrictionsTest.cs:751: if (CheckPermession.CheckUserPageEventEdit(LoginDetails.GroupID, 35))

### 05_CORE_SOURCE_REPORT.txt — line 568

566: GTSErpSystem\Frms\Account\FrmDailyRestrictions.cs:953: if (CheckPermession.CheckUserPage(LoginDetails.GroupID, 36))
567: GTSErpSystem\Frms\Account\FrmDailyRestrictions.cs:1397: if (CheckPermession.CheckUserPage(LoginDetails.GroupID, 139))
568: GTSErpSystem\Frms\Account\FrmDailyRestrictionsTest.cs:691: if (CheckPermession.CheckUserPageEventSave(LoginDetails.GroupID, 35))
569: GTSErpSystem\Frms\Account\FrmDailyRestrictionsTest.cs:751: if (CheckPermession.CheckUserPageEventEdit(LoginDetails.GroupID, 35))
570: GTSErpSystem\Frms\Account\FrmDailyRestrictionsTest.cs:821: if (CheckPermession.CheckUserPageEventDelete(LoginDetails.GroupID, 35))

### AlSaqar_Internal_Source_Map.txt — line 106

104: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmCostCenter.resx | 52514 bytes
105: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmCostCenterTree.resx | 48306 bytes
106: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmDailyRestrictions.resx | 120289 bytes
107: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmDailyRestrictionsTest.resx | 115082 bytes
108: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmDefualtAccount.resx | 76316 bytes

### AlSaqar_Internal_Source_Map.txt — line 107

105: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmCostCenterTree.resx | 48306 bytes
106: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmDailyRestrictions.resx | 120289 bytes
107: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmDailyRestrictionsTest.resx | 115082 bytes
108: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmDefualtAccount.resx | 76316 bytes
109: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmDefualtCustomer.resx | 63276 bytes

### AlSaqar_Internal_Source_Map.txt — line 112

110: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmOpenAccount.resx | 44546 bytes
111: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmPayment.resx | 88665 bytes
112: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmPaymentBig.resx | 138106 bytes
113: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmProjects.resx | 78070 bytes
114: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmReceipts.resx | 92023 bytes

### AlSaqar_Internal_Source_Map.txt — line 115

113: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmProjects.resx | 78070 bytes
114: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmReceipts.resx | 92023 bytes
115: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmReceiptsBig.resx | 140207 bytes
116: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.FrmReceiptsScaffolds.resx | 99298 bytes
117: G:\ماجد سوفت\MajedSoft 08-07-2026\App\GTSErpSystem_Source\GTSErpSystem.Frms.Account.Search.FrmSearchAccount.resx | 21963 bytes


## Legacy route map keywords

### MajedSoft-TRUE-Screen-Mapping.txt — line 2

1: ============================================================
2:  MAJEDSOFT - TRUE SCREEN MAPPING
3: ============================================================
4: DATE: 10/01/2026 11:30:08


## Interpretation guardrail

Use exact names and parameters from these excerpts when mapping screens. Do not infer that a form works merely because its caption or type name resolves.
