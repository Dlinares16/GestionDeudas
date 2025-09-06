import { configureStore } from "@reduxjs/toolkit";
import deudasReducer from "../slices/deudasSlice";
import authReducer from "../slices/authSlice";
import usersReducer from "../slices/usersSlice";

export const store = configureStore({
  reducer: {
    deudas: deudasReducer,
    auth: authReducer,
    users: usersReducer,
  },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
