import React, { FC, useEffect } from "react";
import { useCookies } from "react-cookie";
import { useDispatch, useSelector } from "react-redux";
import styled from "styled-components";

import { Button } from "@/components/misc/button";
import { Close } from "@/components/misc/close";
import { State } from "@/state";
import { hidePromo, showPromo } from "@/state/common";
import { Link } from "./link";

export const Promo: FC = () => {
  const show = useSelector<State, boolean>((state) => state.common.showPromo);
  const dispatch = useDispatch();
  const cookieName = "chillicream-promo-workshop-230510";
  const [cookies, setCookie] = useCookies([cookieName]);
  const consentCookieValue = cookies[cookieName];

  const clickDismiss = () => {
    const expires = new Date();

    expires.setFullYear(new Date().getFullYear() + 1);

    setCookie(cookieName, "true", { path: "/", expires });
  };

  useEffect(() => {
    if (consentCookieValue === "true") {
      dispatch(hidePromo());
    } else {
      dispatch(showPromo());
    }
  }, [consentCookieValue]);

  return (
    <Dialog
      role="dialog"
      aria-live="polite"
      aria-label="promo"
      aria-describedby="promo:desc"
      show={show}
    >
      <Boundary>
        <Container>
          <Message id="promo:desc">
            <Title>Fullstack GraphQL</Title>
            <Description>
              Learn to build modern APIs like Facebook and Netflix in our
              Fullstack GraphQL workshop.
            </Description>
          </Message>
          <Actions>
            <Tickets to="https://www.eventbrite.com/e/fullstack-graphql-tickets-583856048157">
              Get tickets!
            </Tickets>
            <Dismiss
              aria-label="dismiss promo message"
              onClick={clickDismiss}
            />
          </Actions>
        </Container>
      </Boundary>
    </Dialog>
  );
};

const Dialog = styled.div<{ show: boolean }>`
  position: fixed;
  bottom: 0;
  z-index: 40;
  display: ${({ show }) => (show ? "initial" : "none")};
  width: 100vw;
  background-color: #ffb806;
  opacity: ${({ show }) => (show ? 1 : 0)};
  transition: opacity 0.2s ease-in-out;
`;

const Boundary = styled.div`
  position: relative;
  max-width: 1000px;
  margin: auto;
`;

const Container = styled.div`
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 15px 20px;

  @media only screen and (min-width: 600px) {
    flex-direction: row;
    align-items: flex-end;
    gap: 20px;
  }
`;

const Message = styled.div`
  flex: 1 1 auto;
  font-size: 16px;

  @media only screen and (min-width: 600px) {
    font-size: 18px;
  }
`;

const Title = styled.h2`
  color: #4f3903;
`;

const Description = styled.p`
  line-height: 1.667em;
  color: #4f3903;
`;

const Actions = styled.div`
  flex: 0 0 auto;
  display: flex;
  flex-direction: column-reverse;
  align-items: flex-end;
`;

const Tickets = styled(Link)`
  width: 120px;
  padding: 10px 15px;
  margin-bottom: 10px;
  border-radius: var(--border-radius);
  font-size: 0.833em;
  font-weight: 500;
  text-align: center;
  color: #ffffff;

  background-color: #e55723;
  transition: background-color 0.2s ease-in-out;

  &:hover {
    background-color: #d1410c;
  }

  @media only screen and (min-width: 600px) {
    margin-right: 10px;
    align-self: flex-end;
  }
`;

const Dismiss = styled(Close)`
  position: absolute;
  top: 10px;
  right: 10px;
  padding: 10px;
`;
