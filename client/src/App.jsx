import { Route, Routes, useNavigate, useNavigation, } from "react-router-dom"
import AdminLayout from "./layouts/AdminLayout"
import useRoutesElements from "@/useRouteElements";
import { RolesTable } from "./components/RolesTable";
import { UsersTable } from "./components/UsersTable";
import GeneralLayout from "./layouts";
import Home from "./pages/general/Home";
import StudentContribution from "./pages/client/manage/StudentContribution";
import Login from "./pages/general/Login";
import Profile from "./pages/client/manage/Profile";
import AddContribution from "./pages/client/manage/contribution/AddContribution";

function App() {
  // const routes = useRoutesElements()
  console.log(navigator)
  return (
    <Routes>
      <Route path='/admin/roles' element={<AdminLayout>
        <RolesTable />
      </AdminLayout>} />
      <Route path='/admin/users' element={<AdminLayout><UsersTable /></AdminLayout>} />
      <Route path="/" element={<GeneralLayout><Home></Home></GeneralLayout>}>
      </Route>
      <Route path="/manage/recent" element={<StudentContribution></StudentContribution>}>
      </Route>
      <Route path="/login" element={<Login></Login>}>
      </Route>
      <Route path="/manage/profile" element={<Profile></Profile>}>
      </Route>
      <Route path="/manage/add-contribution" element={<AddContribution></AddContribution>}>
      </Route>
      <Route path="*" element={<div className="flex items-center justify-center w-full min-h-screen m-auto text-4xl font-bold">404</div>}></Route>
    </Routes>
  )
}

export default App
