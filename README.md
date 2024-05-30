# Unity2D_MapleStory

DirectX11 2D 게임으로 만들어 본 Maple Story를 Unity2D로 재구성

제작기간 : 2024-05-21 First Commit ~ ?

05-21 :  1.Player(캐릭터) 추가 Idle, Walk, Jump 애니메이션 (State Machine) 구성 
           2.PlayerScript를 만들어 enum Switch 문을 엮어 FSM 구성 
           3.Player 이동, 점프에 따른 RigidBody2D 적용 
           4.임시로 Ground Object 생성 점프 충돌 적용 
           5.(추가사항) : SpriteSheet를 왼쪽방향으로만 구성 -> Script 내에서 SpriteRenderer.flipX를 사용하여 오른쪽 애니메이션 구성 


05-22 : 1.Player Attack 구현 
          2.레이캐스트와 Layer Mask를 이용한 하향 점프 구현 
          3.Player Prone(엎드리기) Prone Attack(엎드려 공격) 구현 
          4.사다리 타기 구현 
   

05-23 : 1.Json파일과 Unity상에서 직렬화와 역직렬화를 통해 토대 구현 (닉네임 , 경험치 , UI 갱신)
          2.한글 폰트 적용
          3.Dictionary 자료형은 JsonUtility로 역직렬화가 안돼서 Json.Net을 이용 JsonConvert 사용

          * 특이사항 : 경험치 상승 과정에서 레벨업을 해도 UI가 갱신되지 않는 버그 디버깅 중 (내일 이어서 작업할 것) 


05-24 : 1.경험치 상승 과정에서 레벨업을 해도 UI가 갱신되지 않는 버그 갱신 
          2.경험치 데이터 테이블 Lv.1 ~ Lv.30 까지 구현 


05-25 : 1.아이템 데이터 테이블(Json) 구현 
          2.인벤토리 UI 구현 


05-27 : 1.데이터 테이블을 기반으로 Item 클래스 생성 
          2.인벤토리 아이템 Change 구현 
          3.인벤토리(Inventory) Save / Loading 기능 구현 (Json파일로 저장, 불러오기 가능)


05-28 : 1.스탯창(Stats) UI 구현 
          2.레벨업 시 Ability Point를 획득하며, 스탯을 올릴 수 있음
          3.캐릭터의 이름,공격력,HP,MP,STR,DEX,INT,LUK을 json파일로부터 읽어와서 UI와 연동 
          4.스탯 (Stats) Save / Loading 기능 구현 (Json파일로 저장, 불러오기 가능)

05-29 : 1.스킬창 UI 구현
          2.Json파일로부터 데이터를 읽어와서 UI와 연동
          3.퀵슬롯 UI구현

05-30 : 1.인벤토리 아이템과 퀵슬롯 상호작용 구현 
          2.스킬창의 스킬과 퀵슬롯 상호작용 구현
          3.퀵슬롯 내의 슬롯 끼리의 교체작용 구현 
          4.퀵슬롯 내용물 마우스 오른쪽 클릭으로 삭제 기능 구현
          5.이미 있는 아이템이면 놓을 수 없게 구현  
          6. 퀵슬롯 (QuickSlot) Save / Loading 기능 구현 (Json파일로 저장, 불러오기 가능)
          
          * 특이사항 : 퀵슬롯의 SwapItems 함수의 퀵슬롯 저장에서 오류 발생 (디버깅 중)
                           
          
          

오늘 할 것 : 상점, NPC(상호작용)

구현해야 할 것 : 
몬스터(오브젝트 풀링 ,드랍테이블, 드랍), 표창 사용  


구현했지만 추가해야 할 것 :
퀵슬롯 Amount Text
사다리타기 조금 더 보완해야 함 
Attack Animation
스킬(스킬에 따른 능력치 증가량)
스탯(레벨(기본 능력치)과 스탯(추가 능력치)에 따른 증가량)
인벤토리 - Item(ToolTip)
Ground 사선 콜라이더

아이템,스킬 드래그 시 정확히 원하는 마우스 포인트에 얹혀지게?

