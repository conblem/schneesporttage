import { Button } from "ui";
import useSWRParent from "swr";

// @ts-ignore
const defaultFetcher = (url: string) => fetch(url).then(res => res.json());

const useSWR =
  (url: string) => useSWRParent(`https://dev.schneesporttage.ml/api${url}`, defaultFetcher);

export default function Web() {
  const { data, error } = useSWR("/User");
  if (error) return <div>failed to load</div>;
  if (!data) return <div>loading...</div>;

  // render data
  return <div>hello {data.map((user: any) => <p key={user.id}>{user.firstname} {user.lastname}</p>)}</div>;
}
