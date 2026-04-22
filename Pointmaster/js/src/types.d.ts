declare type Patrulje = {
    id: number
    name: string
}

declare type Point = {
    id: number
    points: number
    turnout: number
    patrulje: Patrulje
    post: Post
}

declare type Post = {
    id: number
    name: string
}

declare type PointDTO = {
    point: number
    turnout: number
    patrulje: number
    post: number
}

declare type pointStats = {
    patruljeName: string
    totalPoints: number
    totalTurnout: number
    combinedTotal: number
}

declare type Me = {
    authenticated: boolean
    username: string | null
}