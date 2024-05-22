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
          5.Attack 애니메이션 구현
 
          
내일 해야할 일 
하향점프 보완, 발바닥 콜라이더 플레이어의 자식 X
 
