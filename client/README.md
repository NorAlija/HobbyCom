# Table of Contents
- [Table of Contents](#table-of-contents)
  - [Intro](#intro)
  - [Tech Overview](#tech-overview)
  - [Techniques](#techniques)
    - [Data flow](#data-flow)
  - [Running the application](#running-the-application)
  - [Environment variables](#environment-variables)
  - [What to expect](#what-to-expect)
  - [Todo](#todo)
  - [Improvements and future features](#improvements-and-future-features)


## Intro
HobbyCom is aiming to help users find outdoor workout places, and ability to add and edit info about those places. The app is in its early stages, so only authintication is working for now.

## Tech Overview
 - **ReactNative** was the chosen forntend libary to facilitate implementation by deviding pages into smaller components
 - This **React** app is written in **TypeScript** to insure type saftey and prevent runtime errors
 - **TanStack React Query** is used for handeling data fetching and mutations as well as utilizing caching and query invalidation to insure UI updates

## Techniques
The project tries to follow the "seperation of concerns" prenciple to decopule UI from the business logic for better testability and scalability.

### Data flow
The data flow is managed through custome hooks, which incapsulate the business logic. Within the hooks `React-Query` handles fetching or mutating by calling the endpoint as a mutation or query function, then the 'onSuccess or onError' are handled after. The endpoints are called from a seperate service file with axios. Example:

`handleCreation` function for creating a resource:
```TS
const { mutateAsync: logoutMutation } = useLogout()

const createResource = useCreateResource()
const handleCreation = (e: Event) => {
  e.preventDefault()
  if (requiredFieldsValid) {
    const newEntity: EntityData = {
      /* Normalized fields */
      ordinalPosition: parentCollection.length || 0
    }
    createResource.mutate(newEntity, {
      onSuccess: () => resetState(),
      onError: handleCreationFailure
    })
  }
}

```
`useCreatetaskCard()`:
```TS
function useCreateResource() {
  const queryClient = useQueryClient()
  return useMutation<ResponseType, ErrorType, PayloadType>({
    mutationFn: apiClient.createResourceService,
    onSuccess: () => invalidateParentCollection(), // example queryClient.invalidateQueries({ queryKey: ['lists'] })
    retry: (count, error) => !nonRetryErrors(error) && count < MAX_RETRIES,
    retryDelay: exponentialBackoff
  })
}
```

`Task card service`
```TS
const taskCard: createResourceService = {
    create: async (payload) => {
        const response = await api.post("/resource", payload)
        return validateWithSchema(response)
    }
}
```

## Running the application
Simply after cloning the repo or downloading the project, open the terminal and run:

```sh
CD your-project-dir/client # moves to project directory

yarn # downloads all the necessary packages

yarn start # Runs the app on localhost
```

## Environment variables

Below are the environment variables required for the application:

| Environment Variable                  | Description                       |
| ------------------------------------- | --------------------------------- |
| `EXPO_PUBLIC_DEVELOPMENT_URL_NETWORK` | http://192.xxx.x.xx:5000/api/v1   |
| `EXPO_PUBLIC_API_URL`                 | http://yourdeployedbackend/api/v1 |
| `EXPO_PUBLIC_MODE`                    | development                       |

Variables are saved in a ignored `.env.development` file. Please refer to the `.env.development.example` to see how to setup yours.


## What to expect
- Ability to register, login and logout
- Auto token refresh

## Todo
- Fetch user info
- improve UI
- Implement maps

## Improvements and future features
- To be figured

