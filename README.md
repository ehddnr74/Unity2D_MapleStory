# Unity2D_MapleStory

DirectX11 2D 게임으로 만들어 본 Maple Story를 Unity2D로 재구성

제작기간 : 2024-05-21 First Commit

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
          3.저장해야 할 데이터 저장 불러오기 기능 구현 
          4.Dictionary 자료형은 JsonUtility로 역직렬화가 안돼서 Json.Net을 이용 JsonConvert 사용
          5.경험치 상승 과정에서 레벨업을 해도 UI가 갱신되지 않는 버그 디버깅 중 (내일 이어서 작업할 것) 


05-24 : 1.경험치 상승 과정에서 레벨업을 해도 UI가 갱신되지 않는 버그 갱신 
          2.경험치 데이터 테이블 Lv.1 ~ Lv.30 까지 구현 
         
 
구현해야 할 것 : 
아이템, 인벤토리, 상점, NPC(상호작용), 몬스터(드랍테이블, 드랍), 스탯, 스킬(스킬에 따른 스탯 증가량 표기), 표창 사용  
