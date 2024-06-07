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

05-31 : * 퀵슬롯의 SwapItems 함수의 퀵슬롯 저장 버그 수정 완료
              (교환 할 때 아이콘의 경로를 교환하지 않았었기 때문이었음...) 
          1.상점(Shop) UI 구현 
          2.아이템 데이터 테이블을 가져와 상점 슬롯의 아이콘,가격,이름을 맵핑
          3.상점의 아이템 슬롯 클릭 시 해당하는 슬롯에 클릭되었다는 이미지 출력
          
          -다른 슬롯이 클릭되면 이전 슬롯 클릭 이미지는 지워져야함 (수정할 것)

06-02 : * -다른 슬롯이 클릭되면 이전 슬롯 클릭 이미지는 지워져야함 (수정완료)
          1.상점 Exit 버튼 구매(Buy) 버튼 추가 / 버튼 클릭 시 아이템 구매 가능
          2.Player의 메소와 아이템 데이터테이블의 아이템 가격을 비교 하여 구매할 수 있는지 없는지 여부 확인 후 구매 가능 
          3.표창 아이템은 퀵슬롯에 등록할 수 없도록 예외처리 구현
          4.아이템 구매 시 Player의 돈(Meso) 갱신, 인벤토리 아이템 추가 후 Save / Loading 구현  
          
          - 상점UI에 인벤토리에 존재하는 품목들을 Slot에 나열해 클릭 후 아이템 판매기능 구현해야함

06-03 : 1.상점UI에 인벤토리에 존재하는 아이템들을 List에 담아 인벤토리 보유 품목 구현 
          2.인벤토리 변화 플래그가 있을 때 마다 변경된 json파일의 데이터를 읽어와 상점 UI에 인벤토리 품목 순서대로 내용물 갱신 
          3.인벤토리 Remove함수(아이템 수량에 따라 제거) 구현  

           - 아이템 개수가 1개 초과일 경우 아이템 판매 시 상점UI에 수량이 잘 갱신되는데 
             아이템 개수가 1개일 경우 아이템 판매 시 상점 UI에 일정 오류가 있음 (디버깅 중)  
          
           * 디버깅 완료 ( 상점 UI를 갱신하는 부분과 아이템 판매 버튼 클릭 함수 수정)
           4.ShopKeeper(상점) NPC 추가 (Open Shop) 구현 

06-04 : 1.포탈 추가 씬 전환 기반 작업 
          2.Logo씬 , 사냥터 씬 추가 씬 전환 (데이터 DontDestoryOnLoad)구현 

06-05 : 1.몬스터 오브젝트 풀링 구현
          2.사냥터 씬 추가 후 몬스터 배치 작업

06-06 : 1.공격 애니메이션 수정 (공격 딜레이, 애니메이션 시간 등)
          2.표창 오브젝트 풀링 구현

06-07 : 1.Hit Animation 추가 (물리 적용) 
          2.Hit Animation 보완작업 필요 (맞을 때 랜덤 힘으로 작용해서 튕기기 / 다른 애니메이션으로 전환 작업)
          3.표창 발사 시 가까운 적 탐색 (가까운 적 방향으로 날아가기) 보완작업 필요 
          

오늘 할 것 : 퀵슬롯 Amount Text ,  Fading

구현해야 할 것 : 
몬스터(드랍테이블, 드랍)  ,인벤토리 표창 디테일 , Die Animation, 
표창 충전 시스템, 표창 발사 시 가까운 적 탐색


구현했지만 추가해야 할 것 :
NPC 클릭 부분 
퀵슬롯 Amount Text
사다리타기 조금 더 보완해야 함 
Attack Animation
스킬(스킬에 따른 능력치 증가량)
스탯(레벨(기본 능력치)과 스탯(추가 능력치)에 따른 증가량)
인벤토리 - Item(ToolTip)
Ground 사선 콜라이더

아이템,스킬 드래그 시 정확히 원하는 마우스 포인트에 얹혀지게?

