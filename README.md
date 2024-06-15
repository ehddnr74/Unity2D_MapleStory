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

06-09 : 1.표창 발사 시 가까운 적 탐색 (우선공격) 구현 
          2.맵 밖으로 플레이어가 나가지 못하도록 콜라이더 설치 
          3.퀵슬롯(Quick Slot) 슬롯에 해당하는 키(Key) 맵핑
          4..슬롯에 맵핑된 키(Key)를 눌렀을 때 해당하는 아이템(Item) / 스킬(Skill) 사용 구현 
          *** 슬롯안의 아이템을 교체(Swap)해도 키(Key)와 그에 해당하는 아이템(Item) / 스킬(Skill)을 맵핑 


06-10 : 1.퀵슬롯에 스탯창,스킬창,인벤토리,미니맵,줍기,공격,점프 Icon을 배치하여 사용자가 원하는 키와 맵핑할 수 있게 구현 
          *** Icon은 마우스 오른쪽 클릭 시에도 슬롯이 비워지지 않게 끔 예외(return) 처리 (다른건 비워짐)
          2.Player의 점프(Jump), 공격(Attack)을 퀵슬롯의 Key와 맵핑되도록 변경
          3.퀵슬롯 아이템 수량 (Amount) 교체(Swap), 사용(Use) 시 갱신 , 저장 , 로딩 구현 
            *** 소모품만 수량이 나오도록 구현 (UI아이콘,스킬은 text를 비워둠)
          4.소모성 아이템을 모두 사용 시 퀵슬롯에서 자동으로 비워지게 구현 (Remove함수 내에서 flag에 의해 자동 갱신)
          5.아이템 사용 후 상점안의 인벤토리 보유물 갱신이 안되는 버그 수정 (NPC 클릭 시점 Shop.cs의 UpdateShopInventory함수를 호출해 IntentoryItem.cs의 내용을 List에 다시 받아서 순차적으로 보일 수 있도록 재 갱신)
          6.인벤토리의 변화 flag가 있을때마다 인벤토리의 표창들 중에 가장 우선적으로 위치한 표창의 슬롯을 찾아내 표창의 테두리에 애니메이션 효과를 주어 직관적이게 표현 

06-11 : 1.인벤토리 내 표창이 없다면 공격 시 표창이 나갈 수 없게 구현 
          2.헤이스트(이동속도,점프력 증가), 크리티컬 샷(크리티컬확률 증가), 윈드 부스터(공격 속도 증가) 스킬 추가 
          3.표창 구매시 표창 한 통당 수량(Amount = 1000으로 설정) 
          4.표창 소모 시 (ex : 공격 (Attack)시) 수량이 1보다 작거나 같으면 인벤토리 내의 존재하는 다음 표창이 사용되도록 구현 
          
06-12 : 1.인벤토리내에 우선적으로 존재하는 표창 종류에 따른 표창 사용을 오브젝트 풀링으로 구현
             (Item.ID값에 따라 현재 플레이어가 사용할 표창 불린형 (Player currentSuriken)값 변경)
          2.크리티컬 샷, 윈드 부스터 스킬 추가 **(크리티컬 샷은 패시브스킬로 퀵슬롯 등록 불가 , 윈드 부스터는 퀵슬롯 등록 가능)
          3.오브젝트 풀링을 통한 데미지(Damage) 구현 (크리티컬,논크리티컬 구분 구현) 
          4.데미지의 생성 위치(Position)는 Wolrd 공간의 특정 위치를 스크린 공간의 (X,Y) 좌표로 변환하여 월드 좌표의 정확한
            픽셀 위치를 반환하여 설정  
          5.럭키세븐(표창 2회 연속 날리기 스킬) 사용시 두 번째 날라가는 표창은 SecondSuriken Flag를 두어
            첫 번째 표창 타격 이후 두 번째 표창 타격 시 (데미지 생성 위치) 각각 생성 

06-13 : ******** 버프스킬로 증가하는 효과는 플레이어 변수의 Origin값을 받아서 더해줌 
          1.헤이스트 = 마스터 레벨 : 10 (레벨 단위 증가량 : MoveSpeed +0.2, JumpForce + 0.15) 인게임 내 구현 완료 
          2.윈드 부스터 = 마스터 레벨 : 1 (공격속도를 1.2배 증가시킴) 인게임 내 구현 완료 (1.2배의 수치는 변경 가능)  
          3.크리티컬 샷 = 마스터 레벨 : 20 (레벨 단위 증가량 : 크리티컬 확률 +2%) 인게임 내 구현 완료 
          4.플래시 점프 = 마스터레벨 : 1 (점프도중 한번 더 점프하면 도약 가능) (더블점프 도중 공격은 못하도록 구현)
            ****더블 점프 Flag 구현 , Velocity(x,y) 값 조정으로 자연스러운 점프 구현 
          5.럭키세븐 = 마스터레벨 : 15 (레벨 단위 MP소모 증가량, %데미지 구현)
          6.Json파일에 존재하는 플레이어의 크리티컬확률에 따른 타격 시 크리티컬 데미지 or 일반 데미지 구현  
            **** 크리티컬 샷의 레벨에 따라 결정됨 (크리티컬 확률 Save / Loading 구현) 
            **** 크리티컬 시 데미지 2배증가 (증가량 = 임시)
          7.럭키세븐 스킬 사용일 경우와 일반공격일 경우를 구분해서 데미지계산 (표창 오브젝트 풀 매니저에서 구현)
          8.빨간 달팽이(Monster) Hit, Die 로직 구현 

06-14 : 1.플레이어(Player) 피격 시 데미지 표시 (오브젝트 풀링) 구현
          2.스킬 사용시 플레이어(Player)의 HP,MP 상태를 가져와 사용 가능한 상태에서만 사용되게 구현 
          3.인벤토리 내의 사용중인 표창의 개수가 2개 남았을 때 럭키세븐을 사용하면 0개가 되지 않고 
            1개로 바뀌고 다음 표창으로 사용중인 표창이 넘어가고 잔여수량을 다음 표창에서 제거되도록 구현
          4.인벤토리 아이템 툴팁(ToolTip) 구현 

06-15 : 1.스킬 툴팁(ToolTip) 구현
          2.버프 사용시 화면 상단 UI표시 (Icon, 남은 시간)
            **** 이미 사용중인 버프 재사용 시 UI표시 로직 구현 해야함 
   

오늘 할 것 : 미니맵 구현 , 스킬 사용 이펙트 추가 , 레벨 업 이펙트 추가 , 사운드 추가 


구현 목표 : 정말 필요한 부분 외에는 Update를 돌리기보다 Flag를 주어 필요한 순간에만 Update해서 프레임을 높게 유지할 수 있도록 하기 !! 



구현해야 할 것 : 
몬스터(드랍테이블, 드랍, 경험치증가)



구현했지만 추가해야 할 것 : 
사다리타기 조금 더 보완해야 함 
Ground 사선 콜라이더


